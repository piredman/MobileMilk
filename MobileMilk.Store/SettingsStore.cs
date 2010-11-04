using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using MobileMilk.Model;

namespace MobileMilk.Store
{
    public class SettingsStore : ISettingsStore
    {
        #region Members
        
        private const string AuthorizationFrobSettingDefault = "";
        private const string AuthorizationFrobSettingKeyName = "AuthorizationFrobSetting";
        private const string AuthorizationTokenSettingDefault = "";
        private const string AuthorizationTokenSettingKeyName = "AuthorizationTokenSetting";
        private const string AuthorizationPermissionsSettingDefault = "";
        private const string AuthorizationPermissionsSettingKeyName = "AuthorizationPermissionsSetting";
        
        private const string UserIdSettingDefault = "";
        private const string UserIdSettingKeyName = "UserIdSetting";
        private const string UserNameSettingDefault = "";
        private const string UserNameSettingKeyName = "UserNameSetting";
        private const string FullNameSettingDefault = "";
        private const string FullNameSettingKeyName = "FullNameSetting";

        private const bool LocationServiceSettingDefault = false;
        private const string LocationServiceSettingKeyName = "LocationService";
        private const bool PushNotificationSettingDefault = false;
        private const string PushNotificationSettingKeyName = "PushNotification";
                
        private readonly IsolatedStorageSettings _isolatedStore;

        #endregion Members

        #region Constructor(s)

        public SettingsStore()
        {
            this._isolatedStore = IsolatedStorageSettings.ApplicationSettings;

            if (Common.Environment.InEmulator())
                AuthorizationToken = Common.Constants.AuthorizationToken;
        }

        #endregion Constructor(s)

        #region Properties

        public string AuthorizationFrob
        {
            get { return this.GetValueOrDefault(AuthorizationFrobSettingKeyName, AuthorizationFrobSettingDefault); }
            set { this.AddOrUpdateValue(AuthorizationFrobSettingKeyName, value); }
        }

        public string AuthorizationToken
        {
            get { return this.GetValueOrDefault(AuthorizationTokenSettingKeyName, AuthorizationTokenSettingDefault); }
            set { this.AddOrUpdateValue(AuthorizationTokenSettingKeyName, value); }
        }

        public Permissions AuthorizationPermissions
        {
            get
            {
                var storedPermissions = this.GetValueOrDefault(AuthorizationPermissionsSettingKeyName, AuthorizationPermissionsSettingDefault);

                var permissions = Permissions.none;
                if (Enum.IsDefined(typeof(Permissions), storedPermissions))
                    permissions = (Permissions)Enum.Parse(typeof(Permissions), storedPermissions, true);

                return permissions;
            }
            set
            {
                var Permissions = Enum.GetName(typeof(Permissions), value);
                this.AddOrUpdateValue(AuthorizationPermissionsSettingKeyName, Permissions);
            }
        }

        public string AuthorizationPermissionsAsString
        {
            get
            {
                return this.GetValueOrDefault(AuthorizationPermissionsSettingKeyName, AuthorizationPermissionsSettingDefault);
            }
        }

        public string UserId
        {
            get { return this.GetValueOrDefault(UserIdSettingKeyName, UserIdSettingDefault); }
            set { this.AddOrUpdateValue(UserIdSettingKeyName, value); }
        }

        public string UserName
        {
            get { return this.GetValueOrDefault(UserNameSettingKeyName, UserNameSettingDefault); }
            set { this.AddOrUpdateValue(UserNameSettingKeyName, value); }
        }

        public string FullName
        {
            get { return this.GetValueOrDefault(FullNameSettingKeyName, FullNameSettingDefault); }
            set { this.AddOrUpdateValue(FullNameSettingKeyName, value); }
        }

        public bool LocationServiceAllowed
        {
            get { return this.GetValueOrDefault(LocationServiceSettingKeyName, LocationServiceSettingDefault); }
            set { this.AddOrUpdateValue(LocationServiceSettingKeyName, value); }
        }

        public bool SubscribeToPushNotifications
        {
            get { return this.GetValueOrDefault(PushNotificationSettingKeyName, PushNotificationSettingDefault); }
            set { this.AddOrUpdateValue(PushNotificationSettingKeyName, value); }
        }

        #endregion Properties

        #region Methods

        private void AddOrUpdateValue(string key, object value)
        {
            bool valueChanged = false;

            try
            {
                // if new value is different, set the new value.
                if (this._isolatedStore[key] != value)
                {
                    this._isolatedStore[key] = value;
                    valueChanged = true;
                }
            }
            catch (KeyNotFoundException)
            {
                this._isolatedStore.Add(key, value);
                valueChanged = true;
            }
            catch (ArgumentException)
            {
                this._isolatedStore.Add(key, value);
                valueChanged = true;
            }

            if (valueChanged)
            {
                this.Save();
            }
        }

        private T GetValueOrDefault<T>(string key, T defaultValue)
        {
            T value;

            try
            {
                value = (T)this._isolatedStore[key];
            }
            catch (KeyNotFoundException)
            {
                value = defaultValue;
            }
            catch (ArgumentException)
            {
                value = defaultValue;
            }

            return value;
        }

        private void Save()
        {
            this._isolatedStore.Save();
        }

        #endregion Methods
    }
}
