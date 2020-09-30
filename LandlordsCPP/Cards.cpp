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

	//牌型检测
	if (count < 5) {
		switch (count)
		{
		case 1:break;
		case 2:
			if ((arr[0] == 53 && arr[1] == 54) || (arr[1] == 53 && arr[0] == 54)) {
				//王炸
				CM->mCardsEnum = CardsEnum::JJ;
			}
			else if (arr[0] % 13 == arr[1] % 13) {
				//对子
				CM->mCardsEnum = CardsEnum::AA;
			}
			else {
				return false;
			}
			break;
		case 3:
			if (arr[0] % 13 == arr[1] % 13 && arr[0] % 13 == arr[2] % 13) {
				//三张
				CM->mCardsEnum = CardsEnum::AAA;
			}
			else {
				return false;
			}
		case 4:
			if (arr[1] % 13 == arr[2] % 13) {
				if (arr[0] % 13 == arr[1] && arr[3] % 13 == arr[1] % 13) {
					//炸弹
					CM->mCardsEnum = CardsEnum::AAAA;
				}
				else if (arr[0] % 13 == arr[1] % 13 || arr[3] % 13 == arr[1] % 13) {
					//三代一
					CM->mCardsEnum = CardsEnum::AAAB;
				}
				else {
					return false;
				}
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

		//2 3 4 5 6 7 8 9 10 j q k a jocker
		//出现的点数,14种牌，元素为点数出现次数
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

		int startIndex = 0;
		for (int i = 0; i < 14; i++) {
			if (startIndex == 0 && pointArr[i] != 0) {
				startIndex = i;
			}
			CM->A[pointArr[i]]++;
		}

		//先检查是不是单顺
		if (CM->A[1] == count&&pointArr[13]==0) {
			bool b;
			for (int i = startIndex; i < count; i++) {
				if (pointArr[i] == 0) {
					b = false;
					break;
				}
			}
			if (b) {
				CM->mCardsEnum = CardsEnum::ABCDE;
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
