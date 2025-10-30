using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Suncheon.UI
{
    public interface IUIControl
    {
        public void OnSelect();

        public void DeSelect();
    }
}
