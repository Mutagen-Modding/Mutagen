using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Mutagen.Bethesda.Core.Persistance
{
    record SQLiteFormKeyAllocatorFormIDRecord(string EditorID, uint FormID, uint PatcherID);

    record SQLiteFormKeyAllocatorPatcherRecord(uint PatcherID, string PatcherName);

    class SQLiteFormKeyAllocator : DbContext, IFormKeyAllocator
    {
        private readonly IMod Mod;

        private readonly string ConnectionString;

        private readonly uint PatcherID;

        private readonly HashSet<string> AllocatedEditorIDs = new();

        private readonly DbSet<SQLiteFormKeyAllocatorFormIDRecord> FormIDs;

        private readonly DbSet<SQLiteFormKeyAllocatorPatcherRecord> Patchers;

        public SQLiteFormKeyAllocator(IMod mod, string patcherName, string dbPath)
        {
            Mod = mod;

            Console.WriteLine($"Maintaining FormID allocation database in {dbPath}");
            ConnectionString = $"Data Source={dbPath}";

            this.Database.EnsureCreated();

            FormIDs = this.Set<SQLiteFormKeyAllocatorFormIDRecord>();
            Patchers = this.Set<SQLiteFormKeyAllocatorPatcherRecord>();

            PatcherID = GetPatcherID(patcherName);
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

        public FormKey GetNextFormKey()
        {
            lock (Mod)
            {
                var candidateFormID = Mod.NextFormID;
                if (candidateFormID > 0xFFFFFF)
                    throw new OverflowException();

                // TODO maybe track ranges of allocated formIDs to make this go faster?
                lock (this)
                {
                    // should be $"select 1 from FormIDs where FormID = {candidateFormID}"
                    while (FormIDs.Any(r => r.FormID == candidateFormID)) {
                        candidateFormID++;
                        if (candidateFormID > 0xFFFFFF)
                            throw new OverflowException();
                    }
                }

                Mod.NextFormID = candidateFormID + 1;

                return new FormKey(Mod.ModKey, candidateFormID);
            }
        }

        public FormKey GetNextFormKey(string? editorID)
        {
            if (editorID == null) return GetNextFormKey();

            lock (AllocatedEditorIDs)
                if(!AllocatedEditorIDs.Add(editorID))
                    throw new ConstraintException($"Attempted to allocate a duiplicate unique FormKey for {editorID}");

            SQLiteFormKeyAllocatorFormIDRecord? rec;

            lock (this)
            {
                // should be $"select EditorID, FormID, PatcherID from FormIDs where EditorID = {editorID}"
                rec = FormIDs.AsNoTracking().FirstOrDefault(r => r.EditorID == editorID);
            }

            if (rec is not null)
            {
                if (rec.PatcherID != PatcherID)
                    throw new ConstraintException($"Attempted to allocate a unique FormKey for {editorID} when it was previously allocated by {GetPatcherName(rec.PatcherID)}");
                return Mod.ModKey.MakeFormKey(rec.FormID);
            }

            var formKey = GetNextFormKey();

            lock (this)
            {
                FormIDs.Add(new(editorID, formKey.ID, PatcherID));
            }

            return formKey;
        }

        private uint GetPatcherID(string patcherName)
        {
            lock (this)
            {
                // TODO figure out how to get EF to generate this statement.
                this.Database.ExecuteSqlInterpolated($"insert or ignore into Patchers(PatcherName) values ({patcherName})");

                // should be $"select PatcherID from Patchers where PatcherName = {patcherName}"
                return this.Patchers.Where(r => r.PatcherName == patcherName).Select(r => r.PatcherID).Single();
            }
        }

        private object GetPatcherName(uint patcherID)
        {
            lock (this)
            {
                // should be $"select PatcherName from Patchers where PatcherID = {patcherID}"
                return Patchers.Where(r => r.PatcherID == patcherID).Select(r => r.PatcherName);
            }
        }

        public void ClearPatcher() => ClearPatcher(PatcherID);

        public void ClearPatcher(string patcherName) => ClearPatcher(GetPatcherID(patcherName));

        private void ClearPatcher(uint patcherID)
        {
            lock (this)
            {
                // should be $"delete from FormIDs where PatcherID = {patcherID}", but probably isn't.
                FormIDs.RemoveRange(FormIDs.Where(r => r.PatcherID == patcherID));
            }
        }

        public void Commit()
        {
            lock (this)
            {
                SaveChanges();
            }
        }
    }
}
