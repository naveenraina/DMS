using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.WebUI.Logic
{
    public static class Permission
    {        
        public static bool GetPermission(string permissionString, UserPermission permission)
        {
            //To make it simple, we are keeping a fixed position of each permission in a string
            // canEditdoc, canRemoveDoc, canEditCat, canRemoveCat, canEditUser, canRemoveUser

            bool result = false;
            if (string.IsNullOrEmpty(permissionString) || permissionString.Length < 6)
            {
                return result;
            }
            switch (permission)
            {
                case UserPermission.CanEditDocument:
                    result = permissionString.ElementAt(0) == '1';
                    break;
                case UserPermission.CanRemoveDocument:
                    result = permissionString.ElementAt(1) == '1';
                    break;
                case UserPermission.CanEditCategory:
                    result = permissionString.ElementAt(2) == '1';
                    break;
                case UserPermission.CanRemoveCategory:
                    result = permissionString.ElementAt(3) == '1';
                    break;
                case UserPermission.CanEditUser:
                    result = permissionString.ElementAt(4) == '1';
                    break;
                case UserPermission.CanRemoveUser:
                    result = permissionString.ElementAt(5) == '1';
                    break;

            }
            return result;
        }

        public static string GetPermissionString(bool canEditDoc, bool canRemoveDoc, bool canEditCat, bool canRemoveCat, bool canEditUser, bool canRemoveUser)
        {
            return Convert.ToInt32(canEditDoc).ToString() + Convert.ToInt32(canRemoveDoc).ToString()
                + Convert.ToInt32(canEditCat).ToString() + Convert.ToInt32(canRemoveCat).ToString()
                + Convert.ToInt32(canEditUser).ToString() + Convert.ToInt32(canRemoveUser).ToString();
        }

        public enum UserPermission
        {
            CanEditDocument, CanRemoveDocument, CanEditCategory, CanRemoveCategory, CanEditUser, CanRemoveUser
        }

    }
}
