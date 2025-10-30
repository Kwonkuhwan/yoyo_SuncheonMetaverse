using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Suncheon.WebData;

namespace Suncheon.Admin
{
    public class AdminManager : MonoBehaviour
    {
        private static AdminManager instance;
        public static AdminManager Instance => instance;

        private void Awake()
        {
            instance = this;
        }

        private Response_AdminLoginResult adminInfo;
        public Response_AdminLoginResult AdminInfo
        {
            get => adminInfo;
            set => adminInfo = value;
        }
    }
}