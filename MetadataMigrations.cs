using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamulosSharePointUpload
{
    public class MetadataMigration
    {

        private string _ReferenceMeta = "";
        private string _SharepointSiteMeta = "";
        private string _UsernameMeta = "";
        private string _PasswordMeta = "";
        private string _LibraryMeta = "";
        private string _QueryFileMeta = "";
        private string _IDMeta = "";


        public string ReferenceMeta
        {
            get { return _ReferenceMeta; }
            set { _ReferenceMeta = value; }
        }
        public string SharepointSiteMeta
        {
            get { return _SharepointSiteMeta; }
            set { _SharepointSiteMeta = value; }
        }
        public string UsernameMeta
        {
            get { return _UsernameMeta; }
            set { _UsernameMeta = value; }
        }
        public string PasswordMeta
        {
            get { return _PasswordMeta; }
            set { _PasswordMeta = value; }
        }
        public string LibraryMeta
        {
            get { return _LibraryMeta; }
            set { _LibraryMeta = value; }
        }
        public string QueryFileMeta
        {
            get { return _QueryFileMeta; }
            set { _QueryFileMeta = value; }
        }
        public string IDMeta
        {
            get { return _IDMeta; }
            set { _IDMeta = value; }
        }

    }
}
