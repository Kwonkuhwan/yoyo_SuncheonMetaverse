using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Suncheon.UI
{
    public interface IButton
    {
        public void Open();
        public void Close();
        public void Click();
    }
}