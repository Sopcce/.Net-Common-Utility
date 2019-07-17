using NUnit.Framework;
using Sop.Common.Img.Gif;
using System;
using System.IO;

namespace Sop.Common.Img.Tests
{
    public class AnimatedGifTest
    {
        private string[] imageFilePaths; 
        private string imageGifPath;
        private string filePath;

        private string outputFilePath;
        private string outputImagePath;
        [SetUp]
        public void Setup()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
           
            filePath = $"{path}Resources\\gif"; 
            imageFilePaths = new String[]
            {
                $"{filePath}\\01.png",
                $"{filePath}\\02.png",
                $"{filePath}\\03.png",
            };
            //ע�⣬����ʹ�ü򵥵�04.gif,��ֻ����3֡��
            //ʹ��05.gif   ����140֡Ŷ
            imageGifPath = $"{filePath}\\04.gif";


            //���Ŀ¼��Ӧ�ô��ڣ�ÿ�ζ����µ����Ŀ¼
            outputFilePath = $"{path}Resources\\gif-output\\";
            if (!Directory.Exists(outputFilePath))
            {
                Directory.CreateDirectory(outputFilePath);
            } 
            var name = Guid.NewGuid().ToString("N");
            outputImagePath = $"{outputFilePath}\\{name}.gif";
             
        }

        /// <summary>
        /// ����gifͼƬ
        /// </summary>
        [Test]
        public void GenerateAminmate_Test()
        {
            foreach (var imgFilePath in imageFilePaths)
            {
                if (!File.Exists(imgFilePath))
                {
                    Assert.False(false, "�ļ�·��������");
                }
            }
            if (File.Exists(outputImagePath))
            {
                File.Delete(outputImagePath);
            }
            var isok = Animate.Instance().GenerateAminmate(imageFilePaths, outputImagePath);
            Assert.IsTrue(isok, "���ɳɹ�");

            var isExists = File.Exists(outputImagePath);
            Assert.IsTrue(isExists, "�ļ�����");

        }

        /// <summary>
        /// �ֽ�gifͼƬ
        /// </summary>
        [Test]
        public void DecomposeAminmate_Test()
        {
            var isExists = File.Exists(imageGifPath);
            Assert.IsTrue(isExists, "�ļ�����");

            var list = Animate.Instance().DecomposeAminmate(imageGifPath, outputFilePath);
            Assert.IsTrue(list.Success, "�ļ��ɹ�");
           
        }

        /// <summary>
        /// ����gifͼƬ
        /// </summary>
        [Test] 
        public void Create_Png_Img_To_Gif_Test()
        {
            foreach (var imgFilePath in imageFilePaths)
            {
                if (!File.Exists(imgFilePath))
                {
                    Assert.IsFalse(false, "�ļ�·��������");
                }
            }
            if (File.Exists(outputImagePath))
            {
                File.Delete(outputImagePath);
            }
            AnimatedGifEncoder e1 = new AnimatedGifEncoder();
            e1.Start(outputImagePath);
            //e1.Delay = 500;    // �ӳټ��
            e1.SetDelay(500);
            e1.SetRepeat(0);  //-1:��ѭ��,0:����ѭ�� ����  
            e1.SetSize(100, 200);
            foreach (var imgFilePath in imageFilePaths)
            {
                e1.AddFrame(System.DrawingCore.Image.FromFile(imgFilePath));
            }
            e1.Finish();
            var isExists = File.Exists(outputImagePath);
            Assert.IsTrue(isExists, "�ļ����ڣ����ɳɹ�");

        }

        /// <summary>
        /// �ֽ�gifͼƬ
        /// </summary>
        [Test]
        public void Create_Gif_Img_To_Png_Test()
        {
            var isExists = File.Exists(imageGifPath);
            Assert.IsTrue(isExists, "�ļ�����");
            AnimatedGifDecoder de = new AnimatedGifDecoder();
            de.Read(imageGifPath);
            for (int i = 0, count = de.GetFrameCount(); i < count; i++)
            {
                System.DrawingCore.Image frame = de.GetFrame(i);

                frame.Save(outputFilePath + Guid.NewGuid().ToString() + ".png");
            }

        }
    }
}