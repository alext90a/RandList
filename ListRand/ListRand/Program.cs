using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListRand
{
    class ListNode
    {
        public ListNode Prev = null;
        public ListNode Next = null;
        public ListNode Rand = null;
        public string Data = "undefined";
    }

    class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        Dictionary<ListNode, int> mNodeIndexes = new Dictionary<ListNode, int>();

        public void Serialize(FileStream fs)
        {
            mNodeIndexes.Clear();
            HashSet<ListNode> visited = new HashSet<ListNode>();
            List<string> saveData = new List<string>();
            LinkedList<ListNode> store = new LinkedList<ListNode>();

            store.AddLast(Head);
            mNodeIndexes.Add(Head, 0);
            while(store.Count !=0)
            {
                ListNode curNode = store.First.Value;
                ListNode curPrev = curNode.Prev;
                ListNode curRand = curNode.Rand;
                ListNode curNext = curNode.Next;
                store.RemoveFirst();
                if(visited.Contains(curNode))
                {
                    continue;
                }

                if(curPrev != null && !visited.Contains(curPrev))
                {
                    store.AddLast(curPrev);
                    if (!mNodeIndexes.ContainsKey(curPrev))
                    {
                        mNodeIndexes.Add(curPrev, mNodeIndexes.Count);
                    }
                }
                if(curRand != null && !visited.Contains(curRand))
                {
                    store.AddLast(curRand);
                    if (!mNodeIndexes.ContainsKey(curRand))
                    {
                        mNodeIndexes.Add(curRand, mNodeIndexes.Count);
                    }
                }
                if(curNext != null && !visited.Contains(curNext))
                {
                    store.AddLast(curNext);
                    if(!mNodeIndexes.ContainsKey(curNext))
                    {
                        mNodeIndexes.Add(curNext, mNodeIndexes.Count);
                    }
                    
                }
                saveData.Add(getSaveData(curNode));
                visited.Add(curNode);
            }

            for(int i=0; i<saveData.Count; ++i)
            {
                AddNodeInfo(fs, saveData[i]);
            }

        }

        public void Deserialize(FileStream fs)
        {
            mNodeIndexes.Clear();
            var streamReader = new StreamReader(fs, Encoding.UTF8, true);
            List<ListNode> nodes = new List<ListNode>();
            List<string> stringData = new List<string>();
            String line;
            do
            {
                line = streamReader.ReadLine();
                if(line == null)
                {
                    continue;
                }
                nodes.Add(new ListNode());
                stringData.Add(line);
                
            }
            while (line != null);

            for(int j=0; j<stringData.Count; ++j)
            {
                fromString(stringData[j], nodes[j], nodes);
            }

            Head = nodes[0];
            int end = 10;
        }

        protected void AddNodeInfo(FileStream fs, string nodeInfo)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(nodeInfo);
            fs.Write(info, 0, info.Length);
        }

        protected string getSaveData(ListNode node)
        {
            int nextIndex = -1;
            int randIndex = -1;
            int prevIndex = -1;
            if(node.Next != null && mNodeIndexes.ContainsKey(node.Next))
            {
                nextIndex = mNodeIndexes[node.Next];
            }
            if(node.Rand != null && mNodeIndexes.ContainsKey(node.Rand))
            {
                randIndex = mNodeIndexes[node.Rand];
            }
            if(node.Prev != null && mNodeIndexes.ContainsKey(node.Prev))
            {
                prevIndex = mNodeIndexes[node.Prev];
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Node\t");
            sb.Append(node.Data);
            sb.Append("\t");
            sb.Append(prevIndex.ToString());
            sb.Append("\t");
            sb.Append(randIndex.ToString());
            sb.Append("\t");
            sb.Append(nextIndex.ToString());
            sb.Append("\n");
            return sb.ToString();
        }

        protected void fromString(string info, ListNode node, List<ListNode> nodeStore)
        {
            
            int startIndex = 0;
            int endIndex = 0;
            startIndex = info.IndexOf("\t", startIndex);
            endIndex = info.IndexOf("\t", startIndex + 1);
            node.Data = info.Substring(startIndex+1, endIndex - startIndex-1);

            startIndex = endIndex;
            endIndex = info.IndexOf("\t", startIndex + 1);
            int prevId = Convert.ToInt32(info.Substring(startIndex+1, endIndex - startIndex-1));

            startIndex = endIndex;
            endIndex = info.IndexOf("\t", startIndex + 1);
            int randId = Convert.ToInt32(info.Substring(startIndex+1, endIndex - startIndex-1));

            startIndex = endIndex;
            endIndex = info.Count() - 1;
            int nextId = Convert.ToInt32(info.Substring(startIndex+1, endIndex - startIndex));

            
            if(prevId != -1)
            {
                node.Prev = nodeStore[prevId];
            }
            if(randId != -1)
            {
                node.Rand = nodeStore[randId];
            }
            if(nextId != -1)
            {
                node.Next = nodeStore[nextId];
            }
            
        }


    }


    class Program
    {
        static void Main(string[] args)
        {
            ListNode nodet18 = new ListNode();
            ListNode nodet23 = new ListNode();
            ListNode nodet34 = new ListNode();
            ListNode nodet72 = new ListNode();

            nodet18.Data = "t-18";
            nodet18.Next = nodet23;
            nodet18.Rand = nodet72;

            nodet23.Data = "t-23";
            nodet23.Next = nodet34;
            nodet23.Prev = nodet18;
            nodet23.Rand = null;

            nodet34.Data = "t-34";
            nodet34.Prev = nodet23;
            nodet34.Next = nodet72;
            nodet34.Rand = nodet18;

            nodet72.Data = "t-72";
            nodet72.Prev = nodet34;
            nodet34.Next = null;
            nodet34.Rand = nodet18;

            ListRand panzerList = new ListRand();
            panzerList.Head = nodet18;
            panzerList.Tail = nodet72;
            panzerList.Count = 4;

            FileStream fs = File.Create("PanzerList.txt");
            panzerList.Serialize(fs);
            fs.Close();

            fs = File.Open("PanzerList.txt", FileMode.Open);
            ListRand panzerList2 = new ListRand();
            panzerList2.Deserialize(fs);
            fs.Close();

            fs = File.Create("PanzerList2.txt");
            panzerList2.Serialize(fs);
            fs.Close();
            int i = 0;
        }
    }
}
