using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HA4IoT.Ui.UWP.ViewModels
{
    public class MainViewModel : MVVM.BaseViewModel
    {
        #region Properties 
        
        private string _header = "Taverna";

        public string Header
        {
            get { return _header; }
            set { Set(ref _header, value); }
        }

        #endregion Properties 

        public MainViewModel()
        {
        }
    }
}
