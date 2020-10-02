#include "Cards.h"
#include "Util.h"

//单顺子
//从最小点数开始的若干张牌的点数的张数都不为0，则说明该牌型是连续的
//三种顺子都不能连到鬼，startIndex的取值范围都不同
//错误的牌型：  J  Q  K  A LJ RJ
//arr_nums下标：9 10 11  12 13 14
//i = startIndex = 9
//i < startIndex+size = 14 
//所以startIndex+size 必须小于 14

//双顺子
//和上面单顺子类似
//不过范围减半

Cards::Cards(const int num_cards):
	CardsBuffer(num_cards)
{

}

Cards::Cards(Cards* target):
	CardsBuffer(target)
{
	mCardsEnum = target->mCardsEnum;

}

void Cards::MakeEmpty()
{
	CardsBuffer::MakeEmpty();
	mCardsEnum = CardsEnum::ERR;
}


void Cards::ComputeCardsMode()
{
	int size = GetSize();

	//出牌模式
	mCardsEnum = CardsEnum::ERR;

	//各个点数出现的张数
	int* arr_nums = nullptr;
	//0张 1张 2张 3张 4张出现的次数（注意：因为大小王的分开的，所以对王会被当成2张单牌）
	int* arr_times = nullptr;
	//最小点数下标
	int startIndex = 0;
	
	//对打出的牌按照张数进行排序
	//此时相同张数从小到大进行排序
	if (GetSize() > 1) {
		SortNum(true);
	}
	GetNumsAndTimes(arr_nums, arr_times, startIndex);
	
	if (arr_nums == nullptr || arr_times == nullptr) {
		return;
	}

	//2.牌型检测
	if (size < 5) {
		switch (size)
		{
		case 1:
			mCardsEnum = CardsEnum::A;
			break;
		case 2:
			if (arr_nums[13] == 1 && arr_nums[14] == 1) { mCardsEnum = CardsEnum::JJ; }//王炸
			else if (arr_times[2] == 1) { mCardsEnum = CardsEnum::AA; }//对子
			break;
		case 3:
			if (arr_times[3] == 1) { mCardsEnum = CardsEnum::AAA; }//三张
			break;
		case 4:
			if (arr_times[4] == 1) { mCardsEnum = CardsEnum::AAAA; }//炸弹
			else if (arr_times[3] == 1 && arr_times[1] == 1) { mCardsEnum = CardsEnum::AAAB; }//三代一
			break;
		default:
			break;
		}
	}
	else {
		int double_seq_count = static_cast<int>(static_cast<float>(size) / 2.0f);
		int third_seq_count = static_cast<int>(static_cast<float>(size) / 3.0f);
		if (arr_times[1] == size) {
			SortRank(true);
			if (startIndex>0 && startIndex + size <= 13 && Util::IsSequence(arr_nums, startIndex, startIndex + size)) {
				mCardsEnum = CardsEnum::ABCDE;//单顺子
			}
		}
		else if (size % 2 == 0 && arr_times[2] == double_seq_count) {
			SortRank(true);
			if (startIndex > 0 && startIndex + double_seq_count <= 13 && Util::IsSequence(arr_nums, startIndex, startIndex + double_seq_count)) {
				mCardsEnum = CardsEnum::AABBCC;//双顺子
			}
		}
		else if (size % 3 == 0 && arr_times[3] == third_seq_count) {
			SortRank(true);
			if (startIndex > 0 && startIndex + third_seq_count <= 13 && Util::IsSequence(arr_nums, startIndex, startIndex + third_seq_count)) {
				mCardsEnum = CardsEnum::AAABBB;//三顺子
			}
		}
		else {
			if (size == 5) {
				if (arr_times[3] == 1 && arr_times[2] == 1) { mCardsEnum = CardsEnum::AAABB; }//三代二
				else if (arr_times[4] == 1 && arr_times[1] == 1) { mCardsEnum = CardsEnum::AAAAB; }//四代一
			}
			else if (size == 6) {
				if ((arr_times[4] == 1 && arr_times[2] == 1)) { mCardsEnum = CardsEnum::AAABB; }//四代二
				else if (arr_times[4] == 1 && arr_times[1] == 2 && !(arr_nums[14] == 1 && arr_nums[13] == 1)) {
					mCardsEnum = CardsEnum::AAABC;//四代两张单牌
				}
			}
			else if (arr_times[3] >= 2 && size - arr_times[3] * 3 <= arr_times[3] 
				&& !(arr_nums[14] == 1 && arr_nums[13] == 1)) {
				mCardsEnum = CardsEnum::AAABBBCD;//飞机	
			}
		}
	}

	delete arr_nums;
	delete arr_times;
}

bool Cards::Compare(CardsBuffer* A, CardsBuffer* B)
{
	Cards* CA = dynamic_cast<Cards*>(A);
	Cards* CB = dynamic_cast<Cards*>(B);

	CardsEnum modeA = CA->GetCardsMode();
	CardsEnum modeB = CB->GetCardsMode();
	
	//王炸
	if (modeA == CardsEnum::JJ) {
		return true;
	}
	if (modeB == CardsEnum::JJ) {
		return false;
	}
	
	//炸弹
	if (modeA == CardsEnum::AAAA && modeB != CardsEnum::AAAA) {
		return true;
	}
	if (modeA != CardsEnum::AAAA && modeB == CardsEnum::AAAA) {
		return false;
	}
	
	//类型不同
	if (modeA != modeB) {
		return false;
	}

	//相同类型但不大于上一出牌
	int ARank = CA->GetBuf()[0];
	int BRank = CB->GetBuf()[0];
	if (ARank > BRank) {
		return true;
	}
	else {
		return false;
	}
}

bool Cards::OutCards(Cards* cards_pre, Cards* cards_last)
{
	//当前位置是Player的手牌
	int count_pre = cards_pre->GetSize();

	if (count_pre == 0) {
		return false;
	}
	//不是正确的牌型
	cards_pre->ComputeCardsMode();
	if (cards_pre->mCardsEnum == CardsEnum::ERR) {
		return false;
	}
	//不满足出牌条件
	if (cards_last != nullptr && !Compare(cards_pre, cards_last)) {
		return false;
	}

	//满足条件，将牌打出
	//出牌之后不需要左移，只需要将没有被打出的牌按照原本的顺序添加到新的数组中
	int oldSize = GetSize();
	const int curCount = oldSize - count_pre;
	int* temp = new int[curCount];
	int tempCount = 0;

	int* buf_pre = cards_pre->GetBuf();
	
	for (int i = 0; i < oldSize; i++) {
		bool isOut = false;
		for (int j = 0; j < count_pre; j++) {
			if (buf_pre[j] == buf[i]) {
				isOut = true;
				break;
			}
		}
		if (!isOut) {
			temp[tempCount++] = buf[i];
		}
	}
	delete buf;
	buf = temp;
	top = tempCount - 1;
	return true;
}

string Cards::GetCardsModeName()
{
	switch (mCardsEnum)
	{
	case CardsEnum::A:
		return "单牌";
	case CardsEnum::JJ:
		return "王炸";
	case CardsEnum::AA:
		return "对子";
	case CardsEnum::AAA:
		return "三条";
	case CardsEnum::AAAA:
		return "炸弹";
	case CardsEnum::AAAB:
		return "三带一";
	case CardsEnum::ABCDE:
		return "顺子";
	case CardsEnum::AAABB:
		return "三带二";
	case CardsEnum::AAAAB:
		return "四带一";
	case CardsEnum::AAAABC:
		return "四带二";
	case CardsEnum::AABBCC:
		return "连对";
	case CardsEnum::AAABBB:
		return "三顺子";
	case CardsEnum::AAABBBCD:
		return "飞机";
	case CardsEnum::ERR:
		return "错误";
	default:
		return "没有找到";
		break;
	}
}