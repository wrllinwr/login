using System;
using System.Collections.Generic;

namespace SFL
{
    public class UpdateInfo
    {
        private List<string> _files = new List<string>();
        private string _folder = string.Empty;
        private int _updateVersion = -1;

        public List<string> Files
        {
            get
            {
                return this._files;
            }
            set
            {
                this._files = value;
            }
        }

        public string Folder
        {
            get
            {
                return this._folder;
            }
            set
            {
                this._folder = value;
            }
        }

        public int UpdateVersion
        {
            get
            {
                return this._updateVersion;
            }
            set
            {
                this._updateVersion = value;
            }
        }
    }
}

