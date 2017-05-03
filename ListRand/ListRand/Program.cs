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

    struct NodeInfo
    {
        public NodeInfo(int index)
        {
            mIndex = index;
            mIsVisited = false;
        }
        public int mIndex;
        public bool mIsVisited;
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
            List<string> saveData = new List<string>();
            Dictionary<ListNode, NodeInfo> nodeIndexes = new Dictionary<ListNode, NodeInfo>();

            //traverse all nodes
            BreadthTraverse(Head, saveData, nodeIndexes);
            string nodeTail = "-1";
            if(Tail != null)
            {
                if (nodeIndexes.ContainsKey(Tail) && nodeIndexes[Tail].mIsVisited)
                {
                    nodeTail = nodeIndexes[Tail].mIndex.ToString();
                }
                else
                {
                    BreadthTraverse(Tail, saveData, nodeIndexes);
                    nodeTail = nodeIndexes[Tail].mIndex.ToString();
                }
            }
            
            //save nodes to file
            addStringToFile(fs, Constants.kCountName + nodeIndexes.Count.ToString() + "\n");
            addStringToFile(fs, Constants.kTailName + nodeTail + "\n");
            for (int i = 0; i < saveData.Count; ++i)
            {
                addStringToFile(fs, saveData[i]);
            }

        }

        void BreadthTraverse(ListNode startNode, List<string> saveData, Dictionary<ListNode, NodeInfo> nodeIndexes)
        {
            LinkedList<ListNode> queue = new LinkedList<ListNode>();
            queue.AddLast(startNode);
            if(!nodeIndexes.ContainsKey(startNode))
            {
                nodeIndexes.Add(startNode, new NodeInfo(nodeIndexes.Count));
            }
            
            while (queue.Count != 0)
            {
                ListNode curNode = queue.First.Value;
                queue.RemoveFirst();
                if (nodeIndexes.ContainsKey(curNode) && nodeIndexes[curNode].mIsVisited)
                {
                    continue;
                }

                addNodeToCheckList(curNode.Prev, queue, nodeIndexes);
                addNodeToCheckList(curNode.Rand, queue, nodeIndexes);
                addNodeToCheckList(curNode.Next, queue, nodeIndexes);
                
                saveData.Add(getSaveDataString(curNode, nodeIndexes));
                NodeInfo info = nodeIndexes[curNode];
                info.mIsVisited = true;
                nodeIndexes[curNode] = info;
            }
        }

        void addNodeToCheckList(ListNode curNode, LinkedList<ListNode> queue, Dictionary<ListNode, NodeInfo> nodeIndexes)
        {
            if (curNode != null)
            {
                queue.AddLast(curNode);
                if (!nodeIndexes.ContainsKey(curNode))
                {
                    nodeIndexes.Add(curNode, new NodeInfo(nodeIndexes.Count));
                }
            }
        }

        protected void addStringToFile(FileStream fs, string nodeInfo)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(nodeInfo);
            fs.Write(info, 0, info.Length);
        }

        protected string getSaveDataString(ListNode node, Dictionary<ListNode, NodeInfo> nodeIndexes)
        {
            int nextIndex = -1;
            int randIndex = -1;
            int prevIndex = -1;
            if (node.Next != null && nodeIndexes.ContainsKey(node.Next))
            {
                nextIndex = nodeIndexes[node.Next].mIndex;
            }
            if (node.Rand != null && nodeIndexes.ContainsKey(node.Rand))
            {
                randIndex = nodeIndexes[node.Rand].mIndex;
            }
            if (node.Prev != null && nodeIndexes.ContainsKey(node.Prev))
            {
                prevIndex = nodeIndexes[node.Prev].mIndex;
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
            ListNode nodet100 = new ListNode();

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

            nodet100.Data = "t-100";
            nodet100.Prev = nodet34;
            nodet100.Rand = nodet23;
            nodet100.Next = null;

            ListRand panzerList = new ListRand();
            
            panzerList.Head = nodet18;
            panzerList.Tail = nodet100;
            

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
