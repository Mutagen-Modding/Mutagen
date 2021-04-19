using Microsoft.EntityFrameworkCore;
using Mutagen.Bethesda.Persistence;
using Noggog;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace Mutagen.Bethesda.Persistence
{
    record SQLiteFormKeyAllocatorFormIDRecord(string EditorID, uint FormID, uint PatcherID);

    record SQLiteFormKeyAllocatorPatcherRecord(uint PatcherID, string PatcherName);

    internal class SQLiteFormKeyAllocatorDbContext : DbContext
    {
        private readonly string _connectionString;

        internal readonly DbSet<SQLiteFormKeyAllocatorFormIDRecord> _formIDs;

        internal readonly DbSet<SQLiteFormKeyAllocatorPatcherRecord> _patchers;

        internal SQLiteFormKeyAllocatorDbContext(string dbPath)
        {
            _connectionString = $"Data Source={dbPath}";

            this.Database.EnsureCreated();

            _formIDs = this.Set<SQLiteFormKeyAllocatorFormIDRecord>();
            _patchers = this.Set<SQLiteFormKeyAllocatorPatcherRecord>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
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

    public class SQLiteFormKeyAllocator : BaseSharedFormKeyAllocator
    {
        public static readonly string DefaultPatcherName = "default";

        private uint _patcherID;

        private SQLiteFormKeyAllocatorDbContext _connection;

        private bool _disposedValue;

        private readonly bool _manyPatchers = true;

        private readonly uint _initialNextFormID;

        public SQLiteFormKeyAllocator(IMod mod, string dbPath)
            : this(mod, dbPath, DefaultPatcherName)
        {
            _manyPatchers = false;
        }

        public SQLiteFormKeyAllocator(IMod mod, string dbPath, string activePatcherName)
            : base(mod, dbPath, activePatcherName)
        {
            _initialNextFormID = mod.NextFormID;
            _connection = new SQLiteFormKeyAllocatorDbContext(dbPath);
            _patcherID = GetOrAddPatcherID(ActivePatcherName);
        }

        public override FormKey GetNextFormKey()
        {
            lock (_connection)
            {
                lock (Mod)
                {
                    var candidateFormID = Mod.NextFormID;
                    if (candidateFormID > 0xFFFFFF)
                        throw new OverflowException();

                    // TODO maybe track ranges of allocated formIDs to make this go faster?

                    // should be $"select 1 from FormIDs where FormID = {candidateFormID}"
                    while (_connection._formIDs.Any(r => r.FormID == candidateFormID))
                    {
                        candidateFormID++;
                        if (candidateFormID > 0xFFFFFF)
                            throw new OverflowException();
                    }

                    Mod.NextFormID = candidateFormID + 1;

                    return new FormKey(Mod.ModKey, candidateFormID);
                }
            }
        }

        protected override FormKey GetNextFormKeyNotNull(string editorID)
        {
            lock (_connection)
            {
                // should be $"select EditorID, FormID, PatcherID from FormIDs where EditorID = {editorID}"
                var rec = _connection._formIDs.AsNoTracking().FirstOrDefault(r => r.EditorID == editorID);

                if (rec is not null)
                {
                    if (_manyPatchers)
                        if (rec.PatcherID != _patcherID)
                            throw new ConstraintException($"Attempted to allocate a unique FormKey for {editorID} when it was previously allocated by {GetPatcherName(rec.PatcherID)}");
                    return Mod.ModKey.MakeFormKey(rec.FormID);
                }

                var formKey = GetNextFormKey();

                _connection._formIDs.Add(new(editorID, formKey.ID, _patcherID));

                return formKey;
            }
        }

        private uint GetOrAddPatcherID(string patcherName)
        {
            lock (_connection)
            {
                // TODO figure out how to get EF to generate this statement.
                _connection.Database.ExecuteSqlInterpolated($"insert or ignore into Patchers(PatcherName) values ({patcherName})");

                // should be $"select PatcherID from Patchers where PatcherName = {patcherName}"
                return _connection._patchers.Where(r => r.PatcherName == patcherName).Select(r => r.PatcherID).Single();
            }
        }

        private uint? GetPatcherID(string patcherName)
        {
            lock (_connection)
            {
                // should be $"select PatcherID from Patchers where PatcherName = {patcherName}"
                return _connection._patchers.Where(r => r.PatcherName == patcherName).Select(r => r.PatcherID).FirstOrDefault();
            }
        }

        private string GetPatcherName(uint patcherID)
        {
            lock (_connection)
            {
                // should be $"select PatcherName from Patchers where PatcherID = {patcherID}"
                return _connection._patchers.Where(r => r.PatcherID == patcherID).Select(r => r.PatcherName).Single();
            }
        }

        public void ClearPatcher() => ClearPatcher(_patcherID);

        public void ClearPatcher(string patcherName)
        {
            uint? patcherID = GetPatcherID(patcherName);
            if (patcherID is null) return;
            ClearPatcher((uint)patcherID);
        }

        private void ClearPatcher(uint patcherID)
        {
            lock (_connection)
            {
                // should be $"delete from FormIDs where PatcherID = {patcherID}", but probably isn't.
                _connection._formIDs.RemoveRange(_connection._formIDs.Where(r => r.PatcherID == patcherID));
            }
        }

        public override void Commit()
        {
            lock (_connection)
            {
                _connection.SaveChanges();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            base.Dispose(disposing);
            if (disposing)
                _connection?.Dispose();
            _disposedValue = true;
        }

        public override void Rollback()
        {
            lock (_connection)
            {
                lock (this.Mod)
                {
                    this.Mod.NextFormID = _initialNextFormID;
                }
                _connection?.Dispose();
                _connection = new SQLiteFormKeyAllocatorDbContext(_saveLocation);
                _patcherID = GetOrAddPatcherID(ActivePatcherName);
            }
        }

        public static bool IsPathOfAllocatorType(string path)
        {
            if (!File.Exists(path)) return false;
            try
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                byte[] header = new byte[16];

                for (int i = 0; i < 16; i++)
                {
                    header[i] = (byte)stream.ReadByte();
                }

                return System.Text.Encoding.UTF8.GetString(header).Contains("SQLite format", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
