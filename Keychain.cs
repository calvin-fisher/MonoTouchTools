using System;
using System.Diagnostics;
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.Security;

namespace MonoTouchTools
{
    public static class Keychain
    {
        static Keychain()
        {
            // You should set these yourself, but this at least ensures that there's a value here.
            Service = "MyService";
            Label = "MyLabel";
        }

        /// <summary>A string to use for all of the app's keychain Service entries</summary>
        public static string Service { get; set; }

        /// <summary>A string to use for all of the app's keychain Label entries</summary>
        public static string Label { get; set; }

        /// <summary>Retrieves a password string from the iOS Keychain.</summary>
        /// <param name="account">The name of the account in the entry.</param>
        /// <param name="label">(optional) What should be in the entry's Label field.</param>
        /// <param name="service">(optional) What should be in the entry's Service field.</param>
        public static string GetGenericPasswordField(string account, string label = null, string service = null)
        {
            try
            {
                if (label == null) label = Label;
                if (service == null) service = Service;
                
                if (string.IsNullOrEmpty(label))
                    throw new ArgumentException("Can't retrieve GenericPassword field - must set Keychain.Service property or pass in service string as an argument");
                if (string.IsNullOrEmpty(service))
                    throw new ArgumentException("Can't retrieve GenericPassword field - must set Keychain.Service property or pass in service string as an argument");

                var query = new SecRecord(SecKind.GenericPassword)
                {
                    Service = service,
                    Label = label,
                    Account = account,
                };
                
                SecStatusCode code;
                var record = SecKeyChain.QueryAsRecord(query, out code);
                if (code == SecStatusCode.ItemNotFound)
                    return null;
                
                if (code != SecStatusCode.Success)
                    throw new Exception(string.Format("Error retrieving {0} keychain record: SecStatusCode.{1}", account, code));
                
                if (record == null || record.ValueData == null)
                    return null;
                
                return NSString.FromData(record.ValueData, NSStringEncoding.UTF8);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }

        /// <summary>Stores a password string in the iOS Keychain.</summary>
        /// <param name="account">The name of the account under which the entry should be saved.</param>
        /// <param name="value">The password value to store.</param>
        /// <param name="label">(optional) What to save in the entry's Label field.</param>
        /// <param name="service">(optional) What to save in the entry's Service field.</param>
        public static void SetGenericPasswordField(string account, string value, string label = null, string service = null)
        {
            try
            {
                if (label == null) label = Label;
                if (service == null) service = Service;

                if (string.IsNullOrEmpty(label))
                    throw new ArgumentException("Can't retrieve GenericPassword field - must set Keychain.Service property or pass in service string as an argument");
                if (string.IsNullOrEmpty(service))
                    throw new ArgumentException("Can't retrieve GenericPassword field - must set Keychain.Service property or pass in service string as an argument");

                // If setting to null, remove record
                if (value == null)
                {
                    var recordToRemove = new SecRecord (SecKind.GenericPassword)
                    { 
                        Service = service, 
                        Label = label,
                        Account = account
                    };
                    
                    var removeCode = SecKeyChain.Remove (recordToRemove);
                    if (removeCode != SecStatusCode.Success && removeCode != SecStatusCode.ItemNotFound)
                        throw new Exception(string.Format("Error removing {0} kechain record: SecStatusCode.{1}", account, removeCode));
                    
                    return;
                }
                
                // First, try saving new record
                var record = new SecRecord (SecKind.GenericPassword) {
                    Service = service, 
                    Label = label, 
                    Account = account,
                    ValueData = NSData.FromString (value),
                };
                
                SecStatusCode code = SecKeyChain.Add (record);
                if (code == SecStatusCode.Success)
                    return;

                // We have a contradiction (error) if it didn't exist before and it didn't not exist before
                if (code != SecStatusCode.DuplicateItem)
                    throw new Exception(string.Format("Error saving new {0} keychain record: SecStatusCode.{1}", account, code));
                
                // Most first attempts will fail because there is an existing record that needs to be removed so we can re-add the new one
                var existingRec = new SecRecord (SecKind.GenericPassword)
                { 
                    Service = service, 
                    Label = label,
                };
                
                code = SecKeyChain.Remove (existingRec);
                if (code != SecStatusCode.Success)
                    throw new Exception(string.Format("Error removing existing {0} kechain record: SecStatusCode.{1}", account, code));

                code = SecKeyChain.Add (record);
                if (code != SecStatusCode.Success)
                    throw new Exception(string.Format("Error saving replacement {0} keychain record: SecStatusCode.{1}", account, code));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }
        }
    }
}