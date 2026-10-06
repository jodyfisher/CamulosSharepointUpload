using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CamulosSharePointUpload
{

    public class MigrationsDatabase
    {
        
        public List<Migration> Migrations = new List<Migration>();
        public List<MetadataMigration> MetadataMigrations = new List<MetadataMigration>();
        
        public MigrationsDatabase()
        {

        }

        //migrator stuff

        public void deleteMigration(Migration m)
        {
            this.Migrations.Remove(m);
        }
        public void Save(string FileName)
        {
            using (var writer = new System.IO.StreamWriter(FileName))
            {
                var serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }
        
        public Migration GetRecord(String recid)
        {
            /// this function return a specfic record in the xml file
            Migration m = new Migration();
            for (int i = 0; i < this.Migrations.Count; i++)
            {
                if (Migrations[i].ID == recid)
                {
                    m = Migrations[i];
                    break;
                }
            }
            return m;
        }
        public void InsertRecord(Migration m)
        {
            
            this.Migrations.Add(m);
        }
        
        
        public void UpdateRecord(Migration dta)
        {
            for (int i = 0; i < this.Migrations.Count; i++)
            {
                if (Migrations[i].ID == dta.ID)
                {
                    Migrations[i] = dta;
                    break;
                }
            }

        }
        

        //metadata migration stuff

        public void deleteMetdataMigration(MetadataMigration m)
        {
            this.MetadataMigrations.Remove(m);
        }
        public void SaveMetdata(string Filename)
        {
            using (var writer = new System.IO.StreamWriter(Filename))
            {
                var serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }

        public MetadataMigration GetRecordMetadata(string recid)
        {
            MetadataMigration m = new MetadataMigration();
            for (int i = 0; i < this.MetadataMigrations.Count; i++)
            {
                if (MetadataMigrations[i].IDMeta == recid)
                {
                    m = MetadataMigrations[i];
                    break;
                }
            }
            return m;
        }
        public void InsertRecordMetadata(MetadataMigration m)
        {

            this.MetadataMigrations.Add(m);
        }
        public void UpdateRecordMetadata(MetadataMigration dta)
        {
            for (int i = 0; i < this.MetadataMigrations.Count; i++)
            {
                if (MetadataMigrations[i].IDMeta == dta.IDMeta)
                {
                    MetadataMigrations[i] = dta;
                    break;
                }
            }

        }

        ///
    }
}
