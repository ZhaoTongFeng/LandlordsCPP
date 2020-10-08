using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LandlordsCS
{
    public enum CardsEnum
    {
        A,
        JJ,//2张牌检测是否相等
        AA,
        AAA,
        AAAA,//4张牌，检测首和尾是否相等
        AAAB,

        //以5为分界线,5张及以上都要先检测顺子
        ABCDE,//先检测单顺子，首先检测是否连续

        AAABB,//倒数第二张是否相同
        AAABC,//倒数第二张是否相同

        AAAAB,

        AAAABB,//四张相同
        AAAABC,//后两张不同

        AABBCC,//所有点数都是对子
        AAABBB,//所有点数都是三张

        AAABBBCD,//3n + n
        ERR
    }
    public class CardsBuf
    {
        public int RANK_COUNT = 15;
        public List<int> buf;
        public int top, maxSize;

        public CardsEnum mCardsMode;

        public CardsBuf(int size)
        {
            buf = new List<int>(size);
            for(int i = 0; i < size; i++)
            {
                buf.Add(0);
            }
            top = -1;
            maxSize = size;
            mCardsMode = CardsEnum.ERR;
        }

        public void Copy(CardsBuf cards)
        {
            if (cards.IsEmpty())
            {
                SetEmpty();
            }
            else
            {
                for(int i = 0; i < cards.top; i++)
                {
                    buf[i] = cards.buf[i];
                }
                top = cards.top;
                maxSize = cards.maxSize;
                mCardsMode = cards.mCardsMode;
            }
        }

        public bool IsEmpty() { return top == -1; }
        public bool IsFull() { return top == maxSize - 1; }
        public void MakeEmpty() { top = -1; }
        public int GetSize() { return top + 1; }
        public int GetMaxSize() { return maxSize; }
        public void SetMax() { top = maxSize - 1; }
        public void SetEmpty() { top = - 1; }


        public bool Push(int x)
        {
            if (IsFull()) { return false; }
            buf[++top] = x;
            return true;
        }

        public bool Pop(ref int x)
        {
            if (IsEmpty())
            {
                x = -1;
                return false;
            }
            else
            {
                x = buf[top--];
                return true;
            }
        }
        public bool Pop(ref CardsBuf cards)
        {
            if (IsEmpty())
            {
                return false;
            }
            else
            {
                cards.Push(buf[top--]);
                return true;
            }
        }
        public bool Pop(ref CardsBuf cards,int n)
        {
            if (GetSize() < n) 
            {
                return false;
            }
            else
            {
                for(int i = 0; i < n; i++)
                {
                    cards.Push(buf[top--]);
                }
                return true;
            }
        }


        public bool Peek(out int x)
        {
            if (IsEmpty()) {
                x = -1;
                return false;
            }
            else
            {
                x = buf[top];
                return true;
            }
        }

        public void ComputeCardsMode()
        {
            int size = GetSize();

            //出牌模式
            mCardsMode = CardsEnum.ERR;


            //各个点数出现的张数
            int[] arr_nums;
            //0张 1张 2张 3张 4张出现的次数（注意：因为大小王的分开的，所以对王会被当成2张单牌）
            int[] arr_times;
            //最小点数下标
            int startIndex;

            //对打出的牌按照张数进行排序
            //此时相同张数从小到大进行排序
            if (size > 1)
            {
                SortNum(true);
            }
            GetNumsAndTimes(out arr_nums, out arr_times, out startIndex);

            //2.牌型检测
            if (size < 5)
            {
                switch (size)
                {
                    case 1:
                        mCardsMode = CardsEnum.A;
                        break;
                    case 2:
                        if (arr_nums[13] == 1 && arr_nums[14] == 1) { mCardsMode = CardsEnum.JJ; }//王炸
                        else if (arr_times[2] == 1) { mCardsMode = CardsEnum.AA; }//对子
                        break;
                    case 3:
                        if (arr_times[3] == 1) { mCardsMode = CardsEnum.AAA; }//三张
                        break;
                    case 4:
                        if (arr_times[4] == 1) { mCardsMode = CardsEnum.AAAA; }//炸弹
                        else if (arr_times[3] == 1 && arr_times[1] == 1) { mCardsMode = CardsEnum.AAAB; }//三代一
                        break;
                    default:
                        break;
                }
            }
            else
            {
                int double_seq_count = (int)(size / 2.0f);
                int third_seq_count = (int)(size / 3.0f);

                if (arr_times[1] == size)
                {
                    SortRank(true);
                    if (startIndex > 0 && startIndex + size <= 13 && IsSequence(arr_nums, startIndex, startIndex + size))
                    {
                        mCardsMode = CardsEnum.ABCDE;//单顺子
                    }
                }
                else if (size % 2 == 0 && arr_times[2] == double_seq_count)
                {
                    SortRank(true);
                    if (startIndex > 0 && startIndex + double_seq_count <= 13 && IsSequence(arr_nums, startIndex, startIndex + double_seq_count))
                    {
                        mCardsMode = CardsEnum.AABBCC;//双顺子
                    }
                }
                else if (size % 3 == 0 && arr_times[3] == third_seq_count)
                {
                    SortRank(true);
                    if (startIndex > 0 && startIndex + third_seq_count <= 13 && IsSequence(arr_nums, startIndex, startIndex + third_seq_count))
                    {
                        mCardsMode = CardsEnum.AAABBB;//三顺子
                    }
                }
                else
                {
                    if (size == 5)
                    {
                        if (arr_times[3] == 1 && arr_times[2] == 1) { mCardsMode = CardsEnum.AAABB; }//三代二
                        else if (arr_times[4] == 1 && arr_times[1] == 1) { mCardsMode = CardsEnum.AAAAB; }//四代一
                    }
                    else if (size == 6)
                    {
                        if ((arr_times[4] == 1 && arr_times[2] == 1)) { mCardsMode = CardsEnum.AAABB; }//四代二
                        else if (arr_times[4] == 1 && arr_times[1] == 2 && !(arr_nums[14] == 1 && arr_nums[13] == 1))
                        {
                            mCardsMode = CardsEnum.AAABC;//四代两张单牌
                        }
                    }
                    else if (arr_times[3] >= 2 && size - arr_times[3] * 3 <= arr_times[3]
                        && !(arr_nums[14] == 1 && arr_nums[13] == 1))
                    {
                        mCardsMode = CardsEnum.AAABBBCD;//飞机	
                    }
                }
            }
        }

        public bool Compare(CardsBuf A,CardsBuf B)
        {
            CardsEnum modeA = A.mCardsMode;
            CardsEnum modeB = B.mCardsMode;

            //王炸
            if (modeA == CardsEnum.JJ)
            {
                return true;
            }
            if (modeB == CardsEnum.JJ)
            {
                return false;
            }

            //一个是炸弹，另一个不是炸弹
            if (modeA == CardsEnum.AAAA && modeB != CardsEnum.AAAA)
            {
                return true;
            }
            if (modeA != CardsEnum.AAAA && modeB == CardsEnum.AAAA)
            {
                return false;
            }

            //类型不同
            if (modeA != modeB)
            {
                return false;
            }

            //相同类型但不大于上一出牌
            int ARank = A.buf[0];
            int BRank = B.buf[0];
            if (ARank > BRank)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool OutCards(ref CardsBuf cards_pre, ref CardsBuf cards_last)
        {
            //当前位置是Player的手牌
            int count_pre = cards_pre.GetSize();

            if (count_pre == 0)
            {
                return false;
            }
            //不是正确的牌型
            cards_pre.ComputeCardsMode();
            if (cards_pre.mCardsMode == CardsEnum.ERR)
            {
                return false;
            }
            //不满足出牌条件
            if (!cards_last.IsEmpty() && !Compare(cards_pre, cards_last))
            {
                return false;
            }

            //满足条件，将牌打出
            //出牌之后不需要左移，只需要将没有被打出的牌按照原本的顺序添加到新的数组中
            int oldSize = GetSize();
            int curCount = oldSize - count_pre;
            List<int> temp = new List<int>(20);
            for(int i = 0; i < 20; i++)
            {
                temp.Add(0);
            }
            int tempCount = 0;

            List<int> buf_pre = cards_pre.buf;

            for (int i = 0; i < oldSize; i++)
            {
                bool isOut = false;
                for (int j = 0; j < count_pre; j++)
                {
                    
                    if (buf_pre[j] == buf[i])
                    {
                        isOut = true;
                        break;
                    }
                }
                if (!isOut)
                {
                    temp[tempCount++] = buf[i];
                }
            }
            buf = temp;
            top = tempCount - 1;
            return true;
        }
        public void Shuffle()
        {
            int indexA, indexB, temp;
            Random random = new Random();
            int size = GetSize();
            for (int i = 0; i < size * 10; i++)
            {
                indexA = random.Next(0, size);
                indexB = random.Next(0, size);
                temp = buf[indexA];
                buf[indexA] = buf[indexB];
                buf[indexB] = temp;
            }
        }

        public void SortRank(bool littleFirst = true)
        {
            int i, j, tmp, last, next, size = GetSize();
            for (i = 0; i < size; i++)
            {
                for (j = size - 1; j > i; j--)
                {
                    last = GetCardRank(buf[j - 1]);
                    next = GetCardRank(buf[j]);
                    if (littleFirst)
                    {
                        if (last < next)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (last > next)
                        {
                            continue;
                        }
                    }
                    tmp = buf[j - 1];
                    buf[j - 1] = buf[j];
                    buf[j] = tmp;
                }
            }
        }

        public void SortNum(bool littleFirst = true)
        {
            //每种扑克的张数
            int[] tmp_arr = new int[RANK_COUNT];
            int size = GetSize();

            for (int i = 0; i < size; i++)
            {
                tmp_arr[GetCardRank(buf[i])]++;
            }
            //扑克的顺序
            int[] tmp_index = new int[RANK_COUNT];

            for (int i = 0; i < RANK_COUNT; i++)
            {
                tmp_index[i] = i;
            }
            //用张数先给扑克排序，优先级
            int tmpa, tmpi;
            for (int i = 0; i < RANK_COUNT; i++)
            {
                for (int j = RANK_COUNT - 1; j > i; j--)
                {
                    if (littleFirst)
                    {
                        if (tmp_arr[j - 1] < tmp_arr[j])
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (tmp_arr[j - 1] > tmp_arr[j])
                        {
                            continue;
                        }
                    }
                    tmpa = tmp_arr[j - 1];
                    tmp_arr[j - 1] = tmp_arr[j];
                    tmp_arr[j] = tmpa;

                    tmpi = tmp_index[j - 1];
                    tmp_index[j - 1] = tmp_index[j];
                    tmp_index[j] = tmpi;

                }
            }
            //最后再用顺序对出牌进行排序
            int tmp, tmp_a = 0, tmp_b = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = size - 1; j > i; j--)
                {
                    //找到两个元素对应的优先级,这地方应该可以用一个二维或者三维数组来记录
                    for (int k = 0; k < RANK_COUNT; k++)
                    {
                        if (GetCardRank(buf[j - 1]) == tmp_index[k])
                        {
                            tmp_a = k;
                        }
                        if (GetCardRank(buf[j]) == tmp_index[k])
                        {
                            tmp_b = k;
                        }
                    }
                    if (littleFirst)
                    {
                        if (tmp_a > tmp_b)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (tmp_a < tmp_b)
                        {
                            continue;
                        }
                    }
                    tmp = buf[j - 1];
                    buf[j - 1] = buf[j];
                    buf[j] = tmp;
                }
            }
        }

        public string GetPrintSource()
        {
            string str = "";
            for (int i = 0; i < GetSize(); i++)
            {
                str += buf[i] + " ";
                
            }
            str += "\n\n";
            return str;
        }

        public string GetPrintName()
        {
            string str = "";
            for (int i = 0; i < GetSize(); i++)
            {
                str += GetCardName(buf[i]);
                str += " ";
            }
            str += "\n\n";
            return str;
        }




        //辅助函数
        public string GetCardsModeName()
        {
            switch (mCardsMode)
            {
                case CardsEnum.A:
                    return "单牌";
                case CardsEnum.JJ:
                    return "王炸";
                case CardsEnum.AA:
                    return "对子";
                case CardsEnum.AAA:
                    return "三条";
                case CardsEnum.AAAA:
                    return "炸弹";
                case CardsEnum.AAAB:
                    return "三带一";
                case CardsEnum.ABCDE:
                    return "顺子";
                case CardsEnum.AAABB:
                    return "三带二";
                case CardsEnum.AAABC:
                    return "三带二";
                case CardsEnum.AAAAB:
                    return "四带一";
                case CardsEnum.AAAABB:
                    return "四带一对";
                case CardsEnum.AAAABC:
                    return "四带二";
                case CardsEnum.AABBCC:
                    return "连对";
                case CardsEnum.AAABBB:
                    return "顺子";
                case CardsEnum.AAABBBCD:
                    return "飞机";
                case CardsEnum.ERR:
                    return "错误";
                default:
                    return "NULL";
            }
        }
        public bool IsSequence(int[] arr_num, int beg, int end)
        {
            for (int i = beg; i < end; i++)
            {
                if (arr_num[i] == 0)
                {
                    return false;
                }
            }
            return true;
        }
        public int GetCardRank(int n)
        {
            if (n == 54)
            {
                return 14;
            }
            else if (n == 53)
            {
                return 13;
            }
            else if (n % 13 == 1)
            {
                return 12;
            }
            else if (n % 13 == 0)
            {
                return 11;
            }
            else
            {
                return n % 13 - 2;
            }
        }
        public static string GetCardName(int n)
        {
            string str;
            if (n == 54)
            {

                str = "RJ";
            }
            else if (n == 53)
            {
                str = "LJ";
            }
            else if (n % 13 == 1)
            {
                str = "A";
            }
            else if (n % 13 == 0)
            {
                str = "K";
            }
            else if (n % 13 == 12)
            {
                str = "Q";
            }
            else if (n % 13 == 11)
            {
                str = "J";
            }
            else
            {
                int x = n % 13;
                str = x.ToString();
            }
            return str;
        }
        public void GetNumsAndTimes(out int[] arr_nums, out int[] arr_times, out int index_beg)
        {
            int size = GetSize();
            arr_nums = new int[RANK_COUNT];
            arr_times = new int[5];
            index_beg = 0;
            for (int i = 0; i < size; i++)
            {
                arr_nums[GetCardRank(buf[i])]++;
            }
            for (int i = 0; i < RANK_COUNT; i++)
            {
                if (index_beg == 0 && arr_nums[i] != 0)
                {
                    index_beg = i;
                }
                arr_times[arr_nums[i]]++;
            }
        }
    }
}
