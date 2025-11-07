#if UNITY_EDITOR
using Framework.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.ED
{
    public class MessageHandler
    {
        struct Message
        {
            public int nCode;
            public string ClassName;
        }
        //------------------------------------------------------
        public static void DoBuilder(string PacketBuilder="Scripts/GameMain/NetWork")
        {
            if (!Directory.Exists(PacketBuilder)) Directory.CreateDirectory(PacketBuilder);

            if (MessageBuilder.MessageTypeEnumType == null)
                return;
            Assembly assembly = null;
            //Dictionary<string, List<Enum>> vEnums = new Dictionary<string, List<Enum>>();
            //HashSet<Enum> vMids = new HashSet<Enum>();
            //foreach (Enum o in Enum.GetValues(MessageBuilder.MessageTypeEnumType))
            //{
            //    string name = o.ToString();
            //    bool bFormat = false;
            //    if(name.EndsWith("Res"))
            //    {
            //        name = name.Substring(0, name.Length - "Res".Length);
            //        bFormat = true;
            //    }
            //    else if (name.EndsWith("Req"))
            //    {
            //        name = name.Substring(0, name.Length - "Req".Length);
            //        bFormat = true;
            //    }
            //    if(bFormat)
            //    {
            //        if(!vEnums.TryGetValue(name, out var vList))
            //        {
            //            vList = new List<Enum>();
            //            vEnums[name] = vList;
            //        }
            //        vList.Add(o);
            //    }
            //}
            //List<Message> vMessageClass = new List<Message>();
            //foreach (var ass in System.AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    assembly = ass;
            //    Type[] types = assembly.GetTypes();
            //    for (int i = 0; i < types.Length; ++i)
            //    {
            //        Type tp = types[i];
            //        if (MessageBuilder.IsGoogleIMessage(tp))
            //        {
            //            int mid = 0;
            //            foreach (var db in vMids)
            //            {
            //                if (tp.FullName.ToLower().Contains(db.ToString().ToLower()))
            //                {
            //                    mid = Convert.ToInt32(db);
            //                    break;
            //                }
            //            }
            //            if (mid == 0)
            //            {
            //                Debug.LogError("消息[" + tp.Name + "] 对应消息码不存在,请按指定格式 xxxReq 或者 xxxRes");
            //                continue;
            //            }
            //            else
            //            {
            //                vMessageClass.Add(new Message() { nCode = (int)mid, ClassName = tp.FullName });
            //            }
            //        }
            //    }
            //}

            Dictionary<int, HashSet<int>> vLockers = new Dictionary<int, HashSet<int>>();
            HashSet<MethodInfo> vMethods = new HashSet<MethodInfo>();
            foreach (var ass in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                assembly = ass;
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    Type tp = types[i];
                    if (tp.IsDefined( typeof(NetHandlerAttribute) ))
                    {
                        MethodInfo[] meths = types[i].GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                        for (int m = 0; m < meths.Length; ++m)
                        {
                            if (meths[m].IsDefined(typeof(NetResponseAttribute), false))
                            {
                                vMethods.Add(meths[m]);
                            }
                            if (meths[m].IsDefined(typeof(NetLockAttribute), false))
                            {
                                NetLockAttribute[] locker = (NetLockAttribute[])meths[m].GetCustomAttributes<NetLockAttribute>();
                                for (int j = 0; j < locker.Length; ++j)
                                {
                                    HashSet<int> vSets;
                                    if (!vLockers.TryGetValue(locker[j].lockID, out vSets))
                                    {
                                        vSets = new HashSet<int>();
                                        vLockers.Add(locker[j].lockID, vSets);
                                    }
                                    vSets.Add(locker[j].unlockID);
                                }
  
                            }
                        }
                    }
                }
            }

            //handler
            {
                string code = "";
                code += "/********************************************************************\n";
                code += "作    者:	" + "自动生成" + "\n";
                code += "描    述:\n";
                code += "*********************************************************************/\n";

                code += "using Framework.Net;\n";
                code += "namespace TopGame.Net\n";
                code += "{\n";
                code += "\tpublic static class NetHandlerRegister\n";
                code += "\t{\n";
                code += "\t\tpublic static void Init(NetHandler handler)\n";
                code += "\t\t{\n";


                HashSet<int> vSets = new HashSet<int>();
                foreach (var method in vMethods)
                {
                    NetResponseAttribute attr = (NetResponseAttribute)method.GetCustomAttribute(typeof(NetResponseAttribute));
                    if (attr == null) continue;

                    if (vSets.Contains(attr.mid))
                    {
                        Debug.LogWarning("具有两个相同的消息码,请检查:[mid=" + attr.mid + "]" + method.DeclaringType.FullName + "::" + method.Name);
                        continue;
                    }

                    vSets.Add(attr.mid);
                    code += "\t\t\thandler.Register(" + attr.mid + "," + method.DeclaringType.FullName + "." + method.Name + ");\r\n";
                }
                code += "\t\t}\n";
                code += "\t}\n";
                code += "}\n";

                string buildDir = Path.Combine(Application.dataPath, PacketBuilder);
                if (!Directory.Exists(buildDir)) Directory.CreateDirectory(buildDir);
                string PacketBuilderPath = Path.Combine(buildDir, "NetHandlerRegister.cs");
                FileStream fs = new FileStream(PacketBuilderPath, FileMode.OpenOrCreate);
                StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
                fs.Position = 0;
                fs.SetLength(0);
                writer.Write(code);
                writer.Close();
            }

            //locker
            {
                string code = "";
                code += "/********************************************************************\n";
                code += "作    者:	" + "自动生成" + "\n";
                code += "描    述:\n";
                code += "*********************************************************************/\n";

                code += "using Framework.Net;\n";
                code += "using System.Collections.Generic;\n";
                code += "namespace TopGame.Net\n";
                code += "{\n";
                code += "\tpublic class NetPacketLocker\n";
                code += "\t{\n";
                //lock
                code += "\t\tpublic static void LockPacket(int mid, HashSet<int> vLockMsg, Dictionary<int, long> vTimerLockMsg)\n";
                code += "\t\t{\n";
                if(vLockers.Count>0)
                {
                    code += "\t\t\tswitch(mid)\r\n";
                    code += "\t\t\t{\r\n";
                    foreach (var method in vLockers)
                    {
                        code += "\t\t\t\tcase " + (int)method.Key + ":{vLockMsg.Add(" + (int)method.Key + "); vTimerLockMsg[mid] = System.DateTime.Now.Ticks; return;}//" + method.Key + "\r\n";
                    }
                    code += "\t\t\t}\r\n";
                }

                code += "\t\t}\n";

                Dictionary<int, HashSet<int>> vUnlock = new Dictionary<int, HashSet<int>>();
                foreach (var method in vLockers)
                {
                    foreach (var ul in method.Value)
                    {
                        HashSet<int> vSet;
                        if(!vUnlock.TryGetValue(ul, out vSet))
                        {
                            vSet = new HashSet<int>();
                            vUnlock.Add(ul, vSet);
                        }
                        vSet.Add(method.Key);
                    }
                }
                //unlock
                code += "\t\tpublic static void UnLockPacket(int mid, HashSet<int> vLockMsg, Dictionary<int, long> vTimerLockMsg)\n";
                code += "\t\t{\n";
                code += "\t\t\tvTimerLockMsg.Remove(mid);\r\n";
                if (vLockers.Count > 0)
                {
                    code += "\t\t\tswitch(mid)\r\n";
                    code += "\t\t\t{\r\n";
                    foreach (var method in vUnlock)
                    {
                        code += "\t\t\t\tcase " + (int)method.Key + "://" + method.Key + "\r\n";
                        foreach (var ul in method.Value)
                        {
                            code += "\t\t\t\tvLockMsg.Remove(" + (int)ul + ");//" + ul + ";\r\n";
                            code += "\t\t\t\tvTimerLockMsg.Remove(" + (int)ul + ");\r\n";
                        }
                        code += "\t\t\t\tbreak;\r\n";
                    }
                    code += "\t\t\t}\r\n";
                }

                code += "\t\t}\n";

                code += "\t}\n";
                code += "}\n";

                string buildDir = Path.Combine(Application.dataPath, PacketBuilder);
                if (!Directory.Exists(buildDir)) Directory.CreateDirectory(buildDir);
                string PacketLockBuilderPath = Path.Combine(Application.dataPath, PacketBuilder, "NetPacketLocker.cs");
                FileStream fs = new FileStream(PacketLockBuilderPath, FileMode.OpenOrCreate);
                StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
                fs.Position = 0;
                fs.SetLength(0);
                writer.Write(code);
                writer.Close();
            }
        }
    }

}
#endif

