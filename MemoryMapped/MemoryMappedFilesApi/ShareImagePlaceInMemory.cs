using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MemoryMappedFilesApiLib
{
    public class ShareImagePlaceInMemory 
    {
        protected string m_mapName = "MapName";
        protected int m_writtenSize = 0;
        protected byte[] m_imageBuffer;
        protected MemoryMappedFile mmf = null;
        int MaxMappedMemorySize = 10 * 1024 * 1024;
        public ShareImagePlaceInMemory(string MapPlace)
        {
            m_mapName = MapPlace;
        }
        public virtual void Create(string fileName)
        {

            MemoryMappedFileSecurity mSec = new MemoryMappedFileSecurity();
            mSec.AddAccessRule(new AccessRule<MemoryMappedFileRights>(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MemoryMappedFileRights.FullControl, AccessControlType.Allow));

            byte[] bytes = File.ReadAllBytes(fileName);
            if (bytes.Length > MaxMappedMemorySize)
            {
                throw (new SystemException("The file is bigger then the allocated memory: " + MaxMappedMemorySize.ToString()));
            }
             
            try
            {

                mmf = MemoryMappedFile.CreateOrOpen("Global\\MmfName", MaxMappedMemorySize, MemoryMappedFileAccess.ReadWrite,
                    MemoryMappedFileOptions.None,
                    mSec,
                    HandleInheritability.Inheritable);
                  
                using (var stream = mmf.CreateViewStream())
                {
                    System.IO.BinaryWriter writer = new System.IO.BinaryWriter(stream);

                    writer.Write(bytes.Length);
                    writer.Write(bytes, 0, bytes.Length);
                }
                m_writtenSize = bytes.Length;
                  
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        public virtual byte [] Read()
        {
            try
            {
                MemoryMappedFile mmf;
                mmf = MemoryMappedFile.OpenExisting("Global\\MmfName");

                
                using (var stream = mmf.CreateViewStream())
                //using (var writer = mmf.CreateViewAccessor(0, m_imageBuffer.Length))
                {
                    System.IO.BinaryReader reader = new System.IO.BinaryReader(stream);
                    byte[] bsize = reader.ReadBytes(4);
                    int size = BitConverter.ToInt32(bsize, 0);
                    m_imageBuffer = reader.ReadBytes(size);
                    return m_imageBuffer;
                    //writer.ReadArray<byte>(0, m_imageBuffer, 0, m_imageBuffer.Length);
                }
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }            
        }
    }

}
