using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Cryptography
{
    public class DigitalSignature
    {
        public static byte[] Create(byte[] message, X509Certificate2 certificate)
        {        
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)certificate.PrivateKey;

            if (csp == null)
            {
                throw new Exception("Valid certificate was not found.");
            }
                    
            byte[] hash = null;
                
            SHA1Managed sha1 = new SHA1Managed();
            hash = sha1.ComputeHash(message);
                      
            byte[] signature = csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
            return signature;
        }


        public static bool Verify(byte[] message, byte[] signature, X509Certificate2 certificate)
        {         
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)certificate.PublicKey.Key;
              
            byte[] hash = null;
          
            SHA1Managed sha1 = new SHA1Managed();
            hash = sha1.ComputeHash(message);
            
            return csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), signature);
        }
    }
}
