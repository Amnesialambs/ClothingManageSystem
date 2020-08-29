using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Windows;

namespace ClothingManageSystem.utils
{
    class FileUtil
    {
        // <summary>
        // 取得指定路径下所有目录及文件名称（可递归）
        // </summary>
        // <param name="strDir">指定路径</param>
        // <param name="strFilePattern">要与 strDir 中的文件名匹配的搜索字符串
        // 例：
        // “*.abc*”返回扩展名为 .abc、.abcd、.abcde、.abcdef 等的文件。
        // “*.abcd”只返回扩展名为 .abcd 的文件。
        // “*.abcde”只返回扩展名为 .abcde 的文件。
        // “*.abcdef”只返回扩展名为 .abcdef 的文件。
        // </param>
        // <param name="arrDirs">查询得到的所有目录ArrayList</param>
        // <param name="arrFiles">查询得到的所有文件名称ArrayList</param>
        // <param name="bIsRecursive">是否递归查询</param>
        public static void GetFileList(string strDir, string strFilePattern, ArrayList arrDirs, ArrayList arrFiles, bool bIsRecursive)
        {

            if (string.IsNullOrEmpty(strDir))
            {
                return;
            }

            try
            {
                //取得指定路径下所有符合条件的文件
                string[] strFiles = Directory.GetFiles(strDir, strFilePattern);
                //取得指定路径下的所有目录
                string[] strDirs = Directory.GetDirectories(strDir);
                foreach (string name in strFiles)
                {
                    arrFiles.Add(name);
                }
                foreach (string name in strDirs)
                {
                    arrDirs.Add(name);
                }
                if (bIsRecursive)
                {
                    //递归
                    if (strDirs.Length > 0)
                    {

                        foreach (string dir in strDirs)
                        {

                            GetFileList(dir, strFilePattern, arrDirs, arrFiles, bIsRecursive);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 异常处理
                MessageBox.Show("获取图片库 异常：" + ex.Message);
            }
        }

        // <summary>
        //  订单数生成目录，再将路径对应文件复制进生成的目录
        // </summary>
        public static string copyFileToPrint(int orderNum, string filePath, string targetPath, string remark)
        {

            string status = "成功";
            try
            {
                // 生成 日期目录
                string dateStr = DateTime.Now.ToString("yyyyMMdd");
                // 生成 订单目录
                targetPath = AddSeparatorChar(targetPath, dateStr);
                targetPath = AddSeparatorChar(targetPath, orderNum.ToString());
                // 目标目录不存在，新建一个
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }
                // 暂停 0.1 秒 避免进程占用问题
                Thread.Sleep(100);

                FileInfo fileInfo = new FileInfo(filePath);
                string targetFileName = Path.GetFileNameWithoutExtension(filePath) + "-" + remark + Path.GetExtension(filePath);
                targetPath = targetPath + System.IO.Path.DirectorySeparatorChar + targetFileName;
                fileInfo.CopyTo(targetPath, true);
            }
            catch (Exception ex)
            {
                status = "失败";
                Console.WriteLine(ex.Message);
            }
            return status;
        }

        private static string AddSeparatorChar(string targetPath, string path)
        {

            if (targetPath[targetPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
            {
                targetPath += System.IO.Path.DirectorySeparatorChar;
            }
            targetPath += path;
            if (!string.IsNullOrEmpty(path))
            {
                AddSeparatorChar(targetPath, "");
            }
            return targetPath;
        }

        public static void Main(string[] args)
        {
            //方法体
            /*string dir = "E:\\222222";
            string strFilePattern = "*.*";
            ArrayList arrDirs = new ArrayList();
            ArrayList arrFiles = new ArrayList();
            bool bIsRecursive = true;
            GetFileList(dir, strFilePattern, arrDirs, arrFiles, bIsRecursive);
            Console.WriteLine(arrDirs.Count + "目录" +arrDirs[0].ToString());*/
            //  Console.WriteLine(arrFiles);

            string dir = "E:\\222222\\110 拷贝";
            string[] dirs = dir.Split('\\');
            foreach (string str in dirs)
            {

                Console.WriteLine(str);
            }
        }

    }
}
