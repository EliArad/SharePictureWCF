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
    public class ShareImagePlaceInFile : ShareImagePlaceInMemory
    {
        string m_lastFileName;
        int fileSize;
        public ShareImagePlaceInFile(string MapPlace) : base (MapPlace)
        {
            m_mapName = MapPlace;
        }
        public static MemoryMappedFile MemFile(string path)
        {
            return MemoryMappedFile.CreateFromFile(
                //include a readonly shared stream
                      File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite),
                      "Global\\MmfName",
                      1024 * 1024,
                      MemoryMappedFileAccess.ReadWrite,
                      null,
                      HandleInheritability.None,
                      false);

        }
        public override void Create(string fileName)
        {
           
            byte[] bytes = File.ReadAllBytes(fileName);
             
            try
            {

                if (mmf != null)
                    mmf.Dispose();
                mmf = MemFile(fileName);

                using (MemoryMappedViewAccessor FileMap = mmf.CreateViewAccessor())
                {
                    fileSize = BitConverter.ToInt32(bytes, 0);
                    FileMap.WriteArray<byte>(0, bytes, 0, bytes.Length);
                }               
                m_lastFileName = fileName;
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        public override byte[] Read()
        {
            try
            {
                MemoryMappedFile mmf;
                mmf = MemoryMappedFile.OpenExisting("Global\\MmfName");


                using (var stream = mmf.CreateViewStream())
                //using (var writer = mmf.CreateViewAccessor(0, m_imageBuffer.Length))
                {
                    System.IO.BinaryReader reader = new System.IO.BinaryReader(stream);

                    m_imageBuffer = reader.ReadBytes(fileSize);
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
