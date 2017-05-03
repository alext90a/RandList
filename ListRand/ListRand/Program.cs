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

        

        public void Serialize(FileStream fs)
        {
            if(Head == null)
            {
                Console.WriteLine("Error: List Rand head is null");
                return;
            }
            HashSet<ListNode> visited = new HashSet<ListNode>();
            List<string> saveData = new List<string>();
            Dictionary<ListNode, int> mNodeIndexes = new Dictionary<ListNode, int>();


            BreadthTraverse(Head, visited, saveData, mNodeIndexes);
            string nodeTail = "-1";
            if(Tail != null)
            {
                if (visited.Contains(Tail))
                {
                    nodeTail = mNodeIndexes[Tail].ToString();
                }
                else
                {
                    BreadthTraverse(Tail, visited, saveData, mNodeIndexes);
                    nodeTail = mNodeIndexes[Tail].ToString();
                }
            }
            

            addStringToFile(fs, Constants.kCountName + mNodeIndexes.Count.ToString() + "\n");
            addStringToFile(fs, Constants.kTailName + nodeTail + "\n");
            for (int i = 0; i < saveData.Count; ++i)
            {
                addStringToFile(fs, saveData[i]);
            }

        }

        void BreadthTraverse(ListNode startNode, HashSet<ListNode> visited, List<string> saveData, Dictionary<ListNode, int> mNodeIndexes)
        {
            LinkedList<ListNode> queue = new LinkedList<ListNode>();
            queue.AddLast(startNode);
            if(!mNodeIndexes.ContainsKey(startNode))
            {
                mNodeIndexes.Add(startNode, mNodeIndexes.Count);
            }
            
            while (queue.Count != 0)
            {
                ListNode curNode = queue.First.Value;
                queue.RemoveFirst();
                if (visited.Contains(curNode))
                {
                    continue;
                }

                addNodeToCheckList(curNode.Prev, visited, queue, mNodeIndexes);
                addNodeToCheckList(curNode.Rand, visited, queue, mNodeIndexes);
                addNodeToCheckList(curNode.Next, visited, queue, mNodeIndexes);
                
                saveData.Add(getSaveDataString(curNode, mNodeIndexes));
                visited.Add(curNode);
            }
        }

        void addNodeToCheckList(ListNode curNode, HashSet<ListNode> visited, LinkedList<ListNode> queue, Dictionary<ListNode, int> mNodeIndexes)
        {
            if (curNode != null && !visited.Contains(curNode))
            {
                queue.AddLast(curNode);
                if (!mNodeIndexes.ContainsKey(curNode))
                {
                    mNodeIndexes.Add(curNode, mNodeIndexes.Count);
                }
            }
        }

        public void Deserialize(FileStream fs)
        {
            var streamReader = new StreamReader(fs, Encoding.UTF8, true);
            List<ListNode> nodes = new List<ListNode>();
            List<string> stringData = new List<string>();
            String curLine;
            curLine = streamReader.ReadLine();
            if(curLine == null)
            {
                Console.WriteLine("Error invalid file!");
                return;
            }
            Count = Convert.ToInt32(curLine.Substring(Constants.kCountName.Length, curLine.Length - Constants.kCountName.Length));
            curLine = streamReader.ReadLine();
            if(curLine == null)
            {
                Console.WriteLine("Error invalid file");
                return;
            }
            int tailInd = Convert.ToInt32(curLine.Substring(Constants.kTailName.Length, curLine.Length - Constants.kTailName.Length));
            do
            {
                curLine = streamReader.ReadLine();
                if (curLine == null)
                {
                    continue;
                }
                nodes.Add(new ListNode());
                stringData.Add(curLine);

            }
            while (curLine != null);

            for (int j = 0; j < stringData.Count; ++j)
            {
                fillNodeFromString(stringData[j], nodes[j], nodes);
            }

            Head = nodes[0];
            if(tailInd == -1)
            {
                Tail = null;
            }
            else
            {
                Tail = nodes[tailInd];
            }
            if(Count != stringData.Count)
            {
                Console.WriteLine("Error element count is not valid");
            }
            
        }

        protected void addStringToFile(FileStream fs, string nodeInfo)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(nodeInfo);
            fs.Write(info, 0, info.Length);
        }

        protected string getSaveDataString(ListNode node, Dictionary<ListNode, int> mNodeIndexes)
        {
            int nextIndex = -1;
            int randIndex = -1;
            int prevIndex = -1;
            if (node.Next != null && mNodeIndexes.ContainsKey(node.Next))
            {
                nextIndex = mNodeIndexes[node.Next];
            }
            if (node.Rand != null && mNodeIndexes.ContainsKey(node.Rand))
            {
                randIndex = mNodeIndexes[node.Rand];
            }
            if (node.Prev != null && mNodeIndexes.ContainsKey(node.Prev))
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

        protected void fillNodeFromString(string infoString, ListNode node, List<ListNode> nodeStore)
        {

            int startIndex = 0;
            int endIndex = 0;
            startIndex = infoString.IndexOf("\t", startIndex);
            endIndex = infoString.IndexOf("\t", startIndex + 1);
            node.Data = infoString.Substring(startIndex + 1, endIndex - startIndex - 1);

            startIndex = endIndex;
            endIndex = infoString.IndexOf("\t", startIndex + 1);
            int prevId = Convert.ToInt32(infoString.Substring(startIndex + 1, endIndex - startIndex - 1));

            startIndex = endIndex;
            endIndex = infoString.IndexOf("\t", startIndex + 1);
            int randId = Convert.ToInt32(infoString.Substring(startIndex + 1, endIndex - startIndex - 1));

            startIndex = endIndex;
            endIndex = infoString.Count() - 1;
            int nextId = Convert.ToInt32(infoString.Substring(startIndex + 1, endIndex - startIndex));


            if (prevId != -1)
            {
                node.Prev = nodeStore[prevId];
            }
            if (randId != -1)
            {
                node.Rand = nodeStore[randId];
            }
            if (nextId != -1)
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
            ListNode nodet54 = new ListNode();

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
            nodet72.Next = null;
            nodet72.Rand = nodet18;

            nodet54.Data = "t-54";
            nodet54.Prev = nodet34;
            nodet54.Rand = nodet23;
            nodet54.Next = null;

            ListRand panzerList = new ListRand();
            
            panzerList.Head = nodet18;
            panzerList.Tail = nodet54;
            

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
        }
    }
}
