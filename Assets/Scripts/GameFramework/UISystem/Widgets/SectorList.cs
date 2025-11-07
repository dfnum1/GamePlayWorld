using System;
using System.Collections;
using UnityEngine;

namespace Framework.UI
{
    public class SectorList : MonoBehaviour
    {
        public class Node<T>
        {
            T data;
            public T Data
            {
                get { return data; }
                set { data = value; }
            }

            public Node<T> Pre;
            public Node<T> Next;
            public Node(T t)
            {
                data = t;
            }
        }

        public class Link<T>
        {
            private int size = 0;
            public int GetLength()
            {
                return size;
            }
            public Node<T> head;
            private Node<T> currentNode;
            public void Add(T t)
            {
                if (head == null)
                {
                    head = new Node<T>(t);
                    currentNode = head;
                }
                else
                {
                    Node<T> node = new Node<T>(t);
                    node.Pre = currentNode;
                    currentNode.Next = node;
                    node.Next = head;
                    currentNode = node;
                    head.Pre = node;

                }
                size++;
                return;
            }

            public Node<T> GetNode(int Id)
            {
                if (Id >= size || Id < 0) return null;
                Node<T> node = head;
                int currentId = 0;
                while (true)
                {
                    if (Id == currentId)
                    {
                        return node;
                    }
                    node = node.Next;
                    currentId++;
                }
            }

            public Node<T> GetPreNode(int Id, int step)
            {
                Node<T> n = GetNode(Id);
                if (n == null) return null;
                int currentId = 0;
                while (true)
                {
                    if (currentId == step)
                    {
                        break;
                    }
                    n = n.Pre;
                    currentId++;
                }
                return n;
            }

            public void Remove(int Index)
            {
                Node<T> n = GetNode(Index);
                if (n == null) return;
                Remove(n);
                return;
            }

            public void Remove(Node<T> node)
            {
                node.Pre.Next = node.Next;
                node.Next.Pre = node.Pre;
                size--;
                return;
            }

            public void Clear()
            {
                head = null;
                currentNode = null;
                size = 0;
            }
        }

        private class ItemData
        {
            public RectTransform R;
            public int IndexId;
            public int PosId;
            public int depth;
            public bool startMove = false;
            public float delay = 0;

            public float CurrentG;
            public float G;
            public Quaternion CurrentRot;
            public Quaternion rot;

            public bool isSelected;
        }


        public GameObject Template;
        private int Num = 7;
        private Link<ItemData> link;
        private void Start()
        {

            // Build(Num);
            return;
        }

        [Range(0, 3600)]
        public float r = 10f;
        [Range(0, 360)]
        public float g = 10;
        private float offsetAg = 0;



        private int centerIndex = 0;
        private int currentSelectedId = 0;
        private float DelayTime = 0;

        public int CurrentSelectId { get { return currentSelectedId; } }

        [Range(1, 10)]
        public float MoveSpeed = 5.0f;
        [SerializeField]
        private Vector3 NormalScale = Vector3.one;
        [SerializeField]
        private Vector3 SelectScale = Vector3.one;

        private Action<RectTransform, int, int> CallBack = null;
        public bool isInited = false;
        private int _num;

        public Action<int> SelectedComplete = null;
        public void Init(int num, int DefaultSelectedId, Action<RectTransform, int, int> callBack = null)
        {
            _num = num;
            Num = num * 2;
            link = new Link<ItemData>();
            int of = Num;
            float ofg = 0;
            if (of % 2 == 0)//偶数转奇数
            {
                of -= 1;
                ofg = g;
            }
            offsetAg = (g * (of - 1)) * 0.5f + ofg;
            centerIndex = (int)(Num * 0.5f);
            CallBack = callBack;
            for (int i = 0; i < Num; i++)
            {
                ItemData itemdata = CreateItemData(i);
                link.Add(itemdata);
                if (i < num)
                {
                    CallBack?.Invoke(itemdata.R, i, i);
                }
                else
                {
                    CallBack?.Invoke(itemdata.R, i - num, i);
                }
            }
            if (!isInited)
            {
                currentSelectedId = DefaultSelectedId;
                StartCoroutine(SetSelecePos(currentSelectedId));
            }
            else
            {
                Reset();
            }
            isInited = true;
            return;
        }
        public void Refresh()
        {
            Node<ItemData> node = link.GetNode(0);
            int index = 0;
            while (true)
            {
                if (node.Data.IndexId < _num)
                {
                    CallBack?.Invoke(node.Data.R, node.Data.IndexId, node.Data.IndexId);
                }
                else
                {
                    CallBack?.Invoke(node.Data.R, node.Data.IndexId - _num, node.Data.IndexId);
                }
                node = node.Next;
                index++;
                if (index == link.GetLength())
                {
                    break;
                }
            }
            return;
        }


        IEnumerator SetSelecePos(int currentSelectedId)
        {
            yield return new WaitForSeconds(0.2f);
            SetSelected(currentSelectedId);
        }

        private ItemData CreateItemData(int i)
        {
            GameObject obj = GameObject.Instantiate(Template.gameObject, transform);
            obj.SetActive(true);
            RectTransform r = obj.GetComponent<RectTransform>();
            r.name = i.ToString();
            ItemData itemdata = new ItemData();
            itemdata.R = r;
            itemdata.IndexId = i;
            itemdata.PosId = i;
            itemdata.CurrentG = getG(i);
            itemdata.CurrentRot = getRot(i);
            itemdata.CurrentG = 0;
            itemdata.delay = i * DelayTime;
            return itemdata;
        }

        //private bool IsNext = false;
        public void ChangeValue(bool isNext)
        {
            Node<ItemData> node = link.GetNode(currentSelectedId);
            if (isNext)
            {
                currentSelectedId = node.Next.Data.IndexId;
            }
            else
            {
                currentSelectedId = node.Pre.Data.IndexId;
            }
            SetSelected(currentSelectedId);
            return;
        }
        public void SelectedIndex(int id)
        {
            if (currentSelectedId == id) return;
            currentSelectedId = id;
            SetSelected(currentSelectedId);
            return;
        }

        private void SetSelected(int indexId)
        {
            // if (node.Data.isSelected)
            {
                SelectedComplete?.Invoke(indexId);
            }

            Node<ItemData> node = link.GetPreNode(indexId, centerIndex);
            int index = 0;
            int total = Num;
            while (true)
            {
                int depth = 0;
                if (index < centerIndex)
                {
                    depth = index;
                }
                else
                {
                    total--;
                    depth = total;
                }

                int id = index;
                if ((node.Data.PosId == Num - 1) && index == 0)
                {//最后一个移到第一个                                
                    node.Data.CurrentG = getG(-1);
                    node.Data.CurrentRot = getRot(-1);
                }
                if (node.Data.PosId == 0 && index == Num - 1)
                {
                    node.Data.CurrentG = getG(Num);
                    node.Data.CurrentRot = getRot(Num);
                }

                //node.Data.R.localScale = NormalScale;
                node.Data.isSelected = false;
                if (node.Data.IndexId == currentSelectedId)
                {
                    node.Data.isSelected = true;
                }

                node.Data.depth = depth;
                node.Data.PosId = id;
                node.Data.G = getG(id);
                node.Data.rot = getRot(id);
                //node.Data.delay = id * DelayTime;
                node.Data.startMove = true;


                node = node.Next;
                index++;
                if (index == link.GetLength())
                {
                    break;
                }
            }
            SetDep();
            moveing = true;
            return;
        }

        private void SetDep()
        {
            Node<ItemData> node = link.GetNode(0);
            int index = 0;
            while (true)
            {
                node.Data.R.SetSiblingIndex(node.Data.depth);
                node = node.Next;
                index++;
                if (index == link.GetLength())
                {
                    break;
                }
            }
        }


        private bool moveing = false;
        void Update()
        {
            if (!moveing || link == null || link.GetLength() <= 0) return;

            Node<ItemData> node = link.GetNode(0);
            int index = 0;
            bool bo = false;
            while (true)
            {
                if (node.Data.startMove)
                {
                    bo = true;
                    node.Data.delay -= Time.deltaTime;
                    if (node.Data.delay <= 0)
                    {
                        node.Data.CurrentRot = Quaternion.Lerp(node.Data.CurrentRot, node.Data.rot, Time.deltaTime * MoveSpeed);
                        node.Data.CurrentG = Mathf.Lerp(node.Data.CurrentG, node.Data.G, Time.deltaTime * MoveSpeed);

                        Vector3 scale = NormalScale;
                        if (node.Data.isSelected)
                        {
                            scale = SelectScale;
                        }
                        node.Data.R.localScale = Vector2.Lerp(node.Data.R.localScale, scale, Time.deltaTime * MoveSpeed);
                        if (Mathf.Abs(node.Data.CurrentG - node.Data.G) <= 0.001f)
                        {
                            node.Data.startMove = false;
                            node.Data.CurrentG = node.Data.G;
                            node.Data.CurrentRot = node.Data.rot;
                            node.Data.delay = node.Data.PosId * DelayTime;
                        }
                        Vector2 v = getPos(node.Data.CurrentG);
                        v.y -= r;

                        node.Data.R.anchoredPosition = v;
                        node.Data.R.rotation = node.Data.CurrentRot;
                    }
                }

                node = node.Next;
                index++;
                if (index == link.GetLength())
                {
                    break;
                }
            }
            moveing = bo;
            // SetPos();
        }

        private void Reset()
        {
            Node<ItemData> node = link.GetNode(0);
            int index = 0;
            while (true)
            {
                node.Data.startMove = false;
                node.Data.CurrentG = getG(node.Data.IndexId);
                node.Data.CurrentRot = getRot(node.Data.IndexId);

                Vector2 v = getPos(node.Data.CurrentG);
                v.y -= r;

                node.Data.R.anchoredPosition = v;
                node.Data.R.rotation = node.Data.CurrentRot;
                node.Data.R.localScale = NormalScale;

                node = node.Next;
                index++;
                if (index == link.GetLength())
                {
                    break;
                }
            }
            SetDep();
            moveing = false;
            return;
        }

        private float getG(int posId)
        {
            return g * posId - offsetAg;// + Rot;
        }

        private Quaternion getRot(int posId)
        {
            return Quaternion.Euler(0, 0, 360 - g * posId + offsetAg);// - Rot);
        }

        private Vector2 getPos(float g)
        {
            g = 90 - g;
            float x = Mathf.Cos(g * Mathf.PI / 180) * r;
            float y = Mathf.Sin(g * Mathf.PI / 180) * r;//* r;
            return new Vector2(x, y);
        }

        private void OnDestroy()
        {
            if (link != null)
            {
                link.Clear();
                link = null;
            }

        }

        public void Clear()
        {
            isInited = false;
            if (link == null) return;
            if (link.GetLength() <= 0)
            {
                link = null;
                return;
            }
            Node<ItemData> node = link.GetNode(0);
            int index = 0;
            while (true)
            {
                GameObject.DestroyImmediate(node.Data.R.gameObject);
                node = node.Next;
                index++;
                if (index == link.GetLength())
                {
                    break;
                }
            }
            link.Clear();
            link = null;
            return;
        }

        public Transform GetItemByIndex(int IndexId)
        {
            if (link == null) return null;
            Node<ItemData> node = link.GetNode(0);
            if (node == null) return null;
            int index = 0;
            while (true)
            {
                if (node.Data.IndexId == index)
                    return node.Data.R;
                node = node.Next;
                index++;
                if (index == link.GetLength())
                {
                    break;
                }
            }
            return null;
        }

        public int GetIndexByItem(GameObject go)
        {
            if (link == null) return -1;
            Node<ItemData> node = link.GetNode(0);
            if (node == null) return -1;
            int index = 0;
            while (true)
            {
                if (node.Data != null && node.Data.R && node.Data.R.gameObject == go)
                    return node.Data.IndexId;
                node = node.Next;
                index++;
                if (index == link.GetLength())
                {
                    break;
                }
            }
            return -1;
        }

        public bool GetIsLoadCompleted()
        {
            return isInited;
        }
    }

}