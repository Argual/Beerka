using System;
using System.Collections.Generic;
using System.Text;

namespace Beerka.Desktop.ViewModel
{
    public class LoginSucceededEventArgs : EventArgs
    {
        public string UserName { get; set; }
        public LoginSucceededEventArgs(string userName) { UserName = userName; }
    }
}
