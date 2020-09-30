#include "Cards.h"

Cards::Cards(const int num_cards):
	BaseCards(num_cards)
{

}

Cards::~Cards()
{

}

void Cards::ComputeAttribute()
{

}

bool Cards::OutCards(int* arr, int count)
{

	if (count == 0) {
		return false;
	}

	CardsMode* CM = new CardsMode();

	//对打出的牌进行排序
	//两种排序
	//一种按照点数从小到大，一种根据张数，从多到少
	if (count > 1) {
		for (int i = 0; i < count; i++) {
			for (int j = count - 1; j > i; j--) {
				if (arr[j - 1] % 13 > arr[j] % 13) {
					int tmp = arr[j - 1];arr[j - 1] = arr[j];arr[j] = tmp;
				}
			}
		}
	}

	//2 3 4 5 6 7 8 9 10 j q k a jocker
	//各个点数出现的张数
	int pointArr[14]{ 0 };

	for (int i = 0; i < count; i++) {
		if (arr[i] == 53 || arr[i] == 54) {
			pointArr[13]++;//JOCKER
		}
		else if (arr[i] % 13 == 1) {
			pointArr[12]++;//A
		}
		else if (arr[i] % 13 == 0) {
			pointArr[11]++;//K
		}
		else {
			pointArr[arr[i] % 13 - 2]++;
		}
	}

	//最小点数下标
	int startIndex = 0;

	for (int i = 0; i < 14; i++) {
		if (startIndex == 0 && pointArr[i] != 0) {
			startIndex = i;
		}
		//0-4张牌出现的次数
		CM->A[pointArr[i]]++;
	}


	//牌型检测
	if (count < 5) {
		switch (count)
		{
		case 1:
			CM->mCardsEnum = CardsEnum::A;
			break;
		case 2:
			if (pointArr[13]==2) {
				//王炸
				CM->mCardsEnum = CardsEnum::JJ;
			}
			else if (CM->A[2]==1) {
				//对子
				CM->mCardsEnum = CardsEnum::AA;
			}
			else {
				return false;
			}
			break;
		case 3:
			if (CM->A[3] == 1) {
				//三张
				CM->mCardsEnum = CardsEnum::AAA;
			}
			else {
				return false;
			}
		case 4:
			if (CM->A[4] == 1) {
				//炸弹
				CM->mCardsEnum = CardsEnum::AAAA;
			}
			else if (CM->A[3] == 1 && CM->A[1] == 1) {
				//三代一
				CM->mCardsEnum = CardsEnum::AAAB;
			}
			else {
				return false;
			}
			break;
		default:
			return false;
		}
	}
	else {
		
		if (CM->A[1] == count) {
			//单顺子
			//从最小点数开始的若干张牌的点数的张数都不为0，则说明该牌型是连续的


			//三种顺子都不能连到鬼，startIndex的取值范围都不同
			//错误的牌型：  J  Q  K  A JK
			//pointArr下标：9 10 11 12 13
			//i = startIndex = 9
			//i < startIndex+count = 14
			//所以startIndex+count 必须小于 14
			if (startIndex + count < 14) {
				return false;
			}
			bool b;
			for (int i = startIndex; i < startIndex+count; i++) {
				if (pointArr[i] == 0) {
					b = false;
					break;
				}
			}
			if (b) {
				CM->mCardsEnum = CardsEnum::ABCDE;
			}
			else {
				return false;
			}
		}
		else if (count % 2 == 0 && CM->A[2] == static_cast<int>(static_cast<float>(count) / 2.0f)) {
			//双顺子
			//和上面单顺子类似
			//不过范围减半

			if (startIndex + CM->A[2] == static_cast<int>(static_cast<float>(count) / 2.0f) < 14) {
				return false;
			}
			bool b;
			for (int i = startIndex; i < static_cast<int>(static_cast<float>(count) / 2.0f); i++) {
				if (pointArr[i] == 0) {
					b = false;
					break;
				}
			}
			if (b) {
				CM->mCardsEnum = CardsEnum::AABBCC;
			}
			else {
				return false;
			}
		}
		else if (count % 3 == 0 && CM->A[3] == static_cast<int>(static_cast<float>(count) / 3.0f)) {
			//三顺子
			if (startIndex + CM->A[3] == static_cast<int>(static_cast<float>(count) / 3.0f) < 14) {
				return false;
			}
			bool b;
			for (int i = startIndex; i < static_cast<int>(static_cast<float>(count) / 3.0f); i++) {
				if (pointArr[i] == 0) {
					b = false;
					break;
				}
			}
			if (b) {
				CM->mCardsEnum = CardsEnum::AAABBB;
			}
			else {
				return false;
			}
		}
		else {
			if (count == 5) {
				if (CM->A[3] == 1 && CM->A[2] == 1&& pointArr[13] != 2) {
					//三代二
					//二不能是对王
					CM->mCardsEnum = CardsEnum::AAABB;
				}
				else if (CM->A[4] == 1 && CM->A[1] == 1) {
					//四代一
					CM->mCardsEnum = CardsEnum::AAAAB;
				}
			}
			else if(count==6&& pointArr[13] != 2){
				if ((CM->A[4] == 1 && CM->A[2] == 1)||(CM->A[4] == 1 && CM->A[1] == 2)) {
					//四代二
					CM->mCardsEnum = CardsEnum::AAABB;
				}
			}
			else if(CM->A[3]>=2&& count - CM->A[3] * 3 <= CM->A[3]&& pointArr[13] != 2){
				//飞机	
				CM->mCardsEnum = CardsEnum::AAABBBCD;
			}
			else {
				return false;
			}
		}
	}


	//是否为回合首次出牌

	//该牌型是否大于前一出牌

	//将牌打出
	const int curCount = cards_count - count;
	int* temp = new int[curCount];
	int tempCount = 0;
	for (int i = 0; i < cards_count; i++) {
		bool isOut = false;
		for (int j = 0; j < count; j++) {
			if (arr[j] == i) {
				isOut = true;
				break;
			}
		}
		if (!isOut) {
			temp[tempCount++] = cards[i];
		}
	}
	delete cards;
	cards = temp;
	cards_count = tempCount;
}
