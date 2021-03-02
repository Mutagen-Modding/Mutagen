using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Mutagen.Bethesda.Core.Persistance
{
    record SQLiteFormKeyAllocatorFormIDRecord(string EditorID, uint FormID, uint PatcherID);

    record SQLiteFormKeyAllocatorPatcherRecord(uint PatcherID, string PatcherName);

    internal class SQLiteFormKeyAllocatorDbContext : DbContext
    {
        private readonly string ConnectionString;

        internal readonly DbSet<SQLiteFormKeyAllocatorFormIDRecord> FormIDs;

        internal readonly DbSet<SQLiteFormKeyAllocatorPatcherRecord> Patchers;

        internal SQLiteFormKeyAllocatorDbContext(string dbPath)
        {
            ConnectionString = $"Data Source={dbPath}";

            this.Database.EnsureCreated();

            FormIDs = this.Set<SQLiteFormKeyAllocatorFormIDRecord>();
            Patchers = this.Set<SQLiteFormKeyAllocatorPatcherRecord>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var formIdRecord = modelBuilder.Entity<SQLiteFormKeyAllocatorFormIDRecord>();
            var patcherRecord = modelBuilder.Entity<SQLiteFormKeyAllocatorPatcherRecord>();

            formIdRecord.ToTable("FormIDs");
            formIdRecord.HasKey(r => r.FormID);
            formIdRecord.HasAlternateKey(r => r.EditorID);

            patcherRecord.ToTable("Patchers");
            patcherRecord.HasKey(r => r.PatcherID);
            patcherRecord.HasAlternateKey(r => r.PatcherName);

            formIdRecord
                .HasOne<SQLiteFormKeyAllocatorPatcherRecord>()
                .WithMany()
                .HasForeignKey(r => r.PatcherID);
        }

    }

    public class SQLiteFormKeyAllocator : IFormKeyAllocator, IDisposable
    {
        private readonly IMod Mod;

        private readonly uint PatcherID;

        private readonly SQLiteFormKeyAllocatorDbContext Connection;

        private bool disposedValue;

        private HashSet<string> AllocatedEditorIDs = new();

        private readonly bool manyPatchers = true;

        public SQLiteFormKeyAllocator(IMod mod, string dbPath) : this(mod, dbPath, "")
        {
            manyPatchers = false;
        }

        public SQLiteFormKeyAllocator(IMod mod, string dbPath, string patcherName)
        {
            Mod = mod;
            Connection = new SQLiteFormKeyAllocatorDbContext(dbPath);
            PatcherID = GetOrAddPatcherID(patcherName);
        }

        public FormKey GetNextFormKey()
        {
            lock (Connection)
            {
                lock (Mod)
                {
                    var candidateFormID = Mod.NextFormID;
                    if (candidateFormID > 0xFFFFFF)
                        throw new OverflowException();

                    // TODO maybe track ranges of allocated formIDs to make this go faster?

                    // should be $"select 1 from FormIDs where FormID = {candidateFormID}"
                    while (Connection.FormIDs.Any(r => r.FormID == candidateFormID)) {
                        candidateFormID++;
                        if (candidateFormID > 0xFFFFFF)
                            throw new OverflowException();
                    }

                    Mod.NextFormID = candidateFormID + 1;

                    return new FormKey(Mod.ModKey, candidateFormID);
                }
            }
        }

        public FormKey GetNextFormKey(string? editorID)
        {
            if (editorID == null) return GetNextFormKey();

            lock (Connection)
            {
                if (!AllocatedEditorIDs.Add(editorID))
                    throw new ConstraintException($"Attempted to allocate a duplicate unique FormKey for {editorID}");

                // should be $"select EditorID, FormID, PatcherID from FormIDs where EditorID = {editorID}"
                var rec = Connection.FormIDs.AsNoTracking().FirstOrDefault(r => r.EditorID == editorID);

                if (rec is not null)
                {
                    if (manyPatchers)
                        if (rec.PatcherID != PatcherID)
                            throw new ConstraintException($"Attempted to allocate a unique FormKey for {editorID} when it was previously allocated by {GetPatcherName(rec.PatcherID)}");
                    return Mod.ModKey.MakeFormKey(rec.FormID);
                }

                var formKey = GetNextFormKey();

                Connection.FormIDs.Add(new(editorID, formKey.ID, PatcherID));
         
                return formKey;
            }
        }

        private uint GetOrAddPatcherID(string patcherName)
        {
            lock (Connection)
            {
                // TODO figure out how to get EF to generate this statement.
                Connection.Database.ExecuteSqlInterpolated($"insert or ignore into Patchers(PatcherName) values ({patcherName})");

                // should be $"select PatcherID from Patchers where PatcherName = {patcherName}"
                return Connection.Patchers.Where(r => r.PatcherName == patcherName).Select(r => r.PatcherID).Single();
            }
        }

        private uint? GetPatcherID(string patcherName)
        {
            lock (Connection)
            {
                // should be $"select PatcherID from Patchers where PatcherName = {patcherName}"
                return Connection.Patchers.Where(r => r.PatcherName == patcherName).Select(r => r.PatcherID).FirstOrDefault();
            }
        }

        private string GetPatcherName(uint patcherID)
        {
            lock (Connection)
            {
                // should be $"select PatcherName from Patchers where PatcherID = {patcherID}"
                return Connection.Patchers.Where(r => r.PatcherID == patcherID).Select(r => r.PatcherName).Single();
            }
        }

        public void ClearPatcher() => ClearPatcher(PatcherID);

        public void ClearPatcher(string patcherName)
        {
            uint? patcherID = GetPatcherID(patcherName);
            if (patcherID is null) return;
            ClearPatcher((uint)patcherID);
        }

        private void ClearPatcher(uint patcherID)
        {
            lock (Connection)
            {
                // should be $"delete from FormIDs where PatcherID = {patcherID}", but probably isn't.
                Connection.FormIDs.RemoveRange(Connection.FormIDs.Where(r => r.PatcherID == patcherID));
            }
        }

        public void Commit()
        {
            lock (Connection)
            {
                Connection.SaveChanges();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;
            if (disposing)
                Connection?.Dispose();
            AllocatedEditorIDs = null!;
            disposedValue = true;
        }

        ~SQLiteFormKeyAllocator()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
