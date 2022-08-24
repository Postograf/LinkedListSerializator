using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedListSerializator
{
    class ListNode
    {
        public ListNode Prev;
        public ListNode Next;
        public ListNode Rand; // произвольный элемент внутри списка
        public string Data;
    }

    class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(FileStream s)
        {
            var idByNode = new Dictionary<ListNode, int>();

            //индексация элементов
            var current = Head;
            for (int i = 0; i < Count; i++)
            {
                idByNode[current] = i;
                current = current.Next;
            }

            //запись в файл (рандомные значения записываются индексами)
            current = Head;
            using (var writer = new BinaryWriter(s))
            {
                foreach (var nodeIdPair in idByNode)
                {
                    writer.Write(nodeIdPair.Key.Data);
                    writer.Write(idByNode[nodeIdPair.Key.Rand]);
                }
            }
        }

        public void Deserialize(FileStream s)
        {
            var nodes = new List<ListNode>();
            var nodesByRandId = new Dictionary<int, List<ListNode>>();

            //считывание нод
            using (var reader = new BinaryReader(s))
            {
                ListNode prev = null;
                ListNode current = null;

                while (reader.PeekChar() != -1)
                {
                    prev = current;
                    current = new ListNode
                    {
                        Prev = prev,
                        Data = reader.ReadString()
                    };

                    if (prev != null)
                        prev.Next = current;

                    nodes.Add(current);
                    var randId = reader.ReadInt32();
                    if (nodesByRandId.ContainsKey(randId) == false)
                        nodesByRandId[randId] = new List<ListNode>();
                    nodesByRandId[randId].Add(current);
                }
            }

            //востановление ссылок на рандомные значения
            foreach (var nodesRandIdPair in nodesByRandId)
            {
                foreach (var node in nodesRandIdPair.Value)
                {
                    node.Rand = nodes[nodesRandIdPair.Key];
                }
            }

            Count = nodes.Count;
            if (Count > 0)
            {
                Head = nodes[0];
                Tail = nodes[nodes.Count - 1];
            }
        }

        public ListRand Add(string data)
        {
            Count++;

            var newNode = new ListNode
            {
                Prev = Tail,
                Data = data
            };

            if (Head == null)
            {
                Head = newNode;
                Tail = Head;
            }
            else
            {
                Tail.Next = newNode;
                Tail = newNode;
            }

            Tail.Rand = RandElement();

            return this;
        }

        public ListRand AddRange(IEnumerable<string> dataList)
        {
            foreach (var data in dataList)
                Add(data);

            return this;
        }

        public ListNode RandElement()
        {
            var rand = new Random().Next(0, Count);
            var res = Head;
            for (int i = 0; i < rand; i++)
            {
                res = res.Next;
            }

            return res;
        }
    }
}
