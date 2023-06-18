using System;
using System.Net;
using System.Runtime.CompilerServices;
    
namespace SFL
{
    public class BabyWebClient : WebClient
    {
        [CompilerGenerated]
        private string _CurrentFile_k__BackingField;
        [CompilerGenerated]
        private string _TargetFile_k__BackingField;

        public string CurrentFile
        {
            [CompilerGenerated]
            get
            {
                return this._CurrentFile_k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this._CurrentFile_k__BackingField = value;
            }
        }

        public string TargetFile
        {
            [CompilerGenerated]
            get
            {
                return this._TargetFile_k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this._TargetFile_k__BackingField = value;
            }
        }
    }
}

