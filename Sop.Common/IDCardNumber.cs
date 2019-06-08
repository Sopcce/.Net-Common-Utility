using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

namespace Sop.Common
{
    /// <summary>
    /// ���֤�������������
    /// </summary>
    public class IdCardNumber
    {
        #region ���֤��Ϣ����

        /// <summary>
        /// ����ʡ����Ϣ
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// ���ڵ�����Ϣ
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// ����������Ϣ
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public DateTime Age { get; set; }

        /// <summary>
        /// �Ա�0ΪŮ��1Ϊ��
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// ���֤����
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// ����Javascript����
        /// </summary>
        public string Json { get; set; }

        #endregion

        #region ��̬����
        /// <summary>
        /// 
        /// </summary>
        private static readonly List<string[]> Areas = new List<string[]>();
        /// <summary>
        /// ��ȡ������Ϣ
        /// </summary>
        private static void FillAreas()
        {
            XmlDocument docXml = new XmlDocument();
            string file = "HostingEnvironment.MapPath(\"~/AreaCodeInfo.xml\")";
            docXml.Load(file);
            XmlNodeList nodelist = docXml.GetElementsByTagName("area");
            foreach (XmlNode node in nodelist)
            {
                string code = node.Attributes["code"].Value;
                string name = node.Attributes["name"].Value;
                IdCardNumber.Areas.Add(new string[] { code, name });
            }
        }
        /// <summary>
        /// �������֤��Ϣ
        /// </summary>
        /// <param name="idCardNumber"></param>
        /// <example>
        ///  IDCardNumber card = IDCardNumber.Get(code);
        /// </example>
        /// <returns>IDCardNumber</returns>
        public static IdCardNumber Get(string idCardNumber)
        {
            if (IdCardNumber.Areas.Count < 1)
                IdCardNumber.FillAreas();
            if (!IdCardNumber.CheckIDCardNumber(idCardNumber))
                throw new Exception("�Ƿ������֤����");
            //
            IdCardNumber cardInfo = new IdCardNumber(idCardNumber);
            return cardInfo;
        }
        /// <summary>
        /// У�����֤�����Ƿ�Ϸ�
        /// </summary>
        /// <param name="idCardNumber"></param>
        /// <returns></returns>
        public static bool CheckIDCardNumber(string idCardNumber)
        {
            //������֤
            Regex rg = new Regex(@"^\d{17}(\d|X)$");
            Match mc = rg.Match(idCardNumber);
            if (!mc.Success) return false;
            //��Ȩ��
            string code = idCardNumber.Substring(17, 1);
            double sum = 0;
            string checkCode = null;
            for (int i = 2; i <= 18; i++)
            {
                sum += int.Parse(idCardNumber[18 - i].ToString(), NumberStyles.HexNumber) * (Math.Pow(2, i - 1) % 11);
            }
            string[] checkCodes ={ "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };
            checkCode = checkCodes[(int)sum % 11];
            if (checkCode != code) return false;
            //
            return true;
        }
        /// <summary>
        /// �������һ�����֤��
        /// </summary>
        /// <returns></returns>
        public static IdCardNumber Radom()
        {
            long tick = DateTime.Now.Ticks;
            return new IdCardNumber(_radomCardNumber((int)tick));
        }
        /// <summary>
        /// �����������֤
        /// </summary>
        /// <param name="count"></param>
        /// <example> 
        /// List<IDCardNumber> list = IDCardNumber.Radom(number);
        /// </example>
        /// <returns></returns>
        public static List<IdCardNumber> Radom(int count)
        {
            List<IdCardNumber> list = new List<IdCardNumber>();
            string cardNumber;
            bool isExits;
            for (int i = 0; i < count; i++)
            {
                do
                {
                    isExits = false;
                    int tick = (int)DateTime.Now.Ticks;
                    cardNumber = IdCardNumber._radomCardNumber(tick * (i + 1));
                    foreach (IdCardNumber c in list)
                    {
                        if (c.CardNumber == cardNumber)
                        {
                            isExits = true;
                            break;
                        }
                    }

                } while (isExits);
                list.Add(new IdCardNumber(cardNumber));
            }
            return list;
        }
        /// <summary>
        /// ���������֤��
        /// </summary>
        /// <param name="seed">���������</param>
        /// <returns></returns>
        private static string _radomCardNumber(int seed)
        {
            if (IdCardNumber.Areas.Count < 1)
                IdCardNumber.FillAreas();
            System.Random rd = new System.Random(seed);
            //������ɷ�֤��
            string area = "";
            do
            {
                area = IdCardNumber.Areas[rd.Next(0, IdCardNumber.Areas.Count - 1)][0];
            } while (area.Substring(4, 2) == "00");
            //�����������
            DateTime birthday = DateTime.Now;
            birthday = birthday.AddYears(-rd.Next(16, 60));
            birthday = birthday.AddMonths(-rd.Next(0, 12));
            birthday = birthday.AddDays(-rd.Next(0, 31));
            //�����
            string code = rd.Next(1000, 9999).ToString("####");
            //�����������֤��
            string codeNumber = area + birthday.ToString("yyyyMMdd") + code;
            double sum = 0;
            string checkCode = null;
            for (int i = 2; i <= 18; i++)
            {
                sum += int.Parse(codeNumber[18 - i].ToString(), NumberStyles.HexNumber) * (Math.Pow(2, i - 1) % 11);
            }
            string[] checkCodes ={ "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };
            checkCode = checkCodes[(int)sum % 11];
            codeNumber = codeNumber.Substring(0, 17) + checkCode;
            //
            return codeNumber;
        }
        #endregion

        #region ���֤��������

        private IdCardNumber(string idCardNumber)
        {
            this.CardNumber = idCardNumber;
            _analysis();
        }
        /// <summary>
        /// �������֤
        /// </summary>
        private void _analysis()
        {
            //ȡʡ�ݣ�����������
            string provCode = CardNumber.Substring(0, 2).PadRight(6, '0');
            string areaCode = CardNumber.Substring(0, 4).PadRight(6, '0');
            string cityCode = CardNumber.Substring(0, 6).PadRight(6, '0');
            for (int i = 0; i < IdCardNumber.Areas.Count; i++)
            {
                if (provCode == IdCardNumber.Areas[i][0])
                    this.Province = IdCardNumber.Areas[i][1];
                if (areaCode == IdCardNumber.Areas[i][0])
                    this.Area = IdCardNumber.Areas[i][1];
                if (cityCode == IdCardNumber.Areas[i][0])
                    this.City = IdCardNumber.Areas[i][1];
                if (Province != null && Area != null && City != null) break;
            }
            //ȡ����
            string ageCode = CardNumber.Substring(6, 8);
            try
            {
                int year = Convert.ToInt16(ageCode.Substring(0, 4));
                int month = Convert.ToInt16(ageCode.Substring(4, 2));
                int day = Convert.ToInt16(ageCode.Substring(6, 2));
                Age = new DateTime(year, month, day);
            }
            catch
            {
                throw new Exception("�Ƿ��ĳ�������");
            }
            //ȡ�Ա�
            string orderCode = CardNumber.Substring(14, 3);
            this.Sex = Convert.ToInt16(orderCode) % 2 == 0 ? 0 : 1;
            //����Json����
            Json = @"prov:'{0}',area:'{1}',city:'{2}',year:'{3}',month:'{4}',day:'{5}',sex:'{6}',number:'{7}'";
            Json = string.Format(Json, Province, Area, City, Age.Year, Age.Month, Age.Day, (Sex == 1 ? "boy" : "gril"), CardNumber);
            Json = "{" + Json + "}";
        }
        #endregion
    }

}
