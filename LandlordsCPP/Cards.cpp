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

	//�Դ�����ƽ�������
	//��������
	//һ�ְ��յ�����С����һ�ָ����������Ӷൽ��
	if (count > 1) {
		for (int i = 0; i < count; i++) {
			for (int j = count - 1; j > i; j--) {
				if (arr[j - 1] % 13 > arr[j] % 13) {
					int tmp = arr[j - 1];arr[j - 1] = arr[j];arr[j] = tmp;
				}
			}
		}
	}

	//���ͼ��
	if (count < 5) {
		switch (count)
		{
		case 1:break;
		case 2:
			if ((arr[0] == 53 && arr[1] == 54) || (arr[1] == 53 && arr[0] == 54)) {
				//��ը
				CM->mCardsEnum = CardsEnum::JJ;
			}
			else if (arr[0] % 13 == arr[1] % 13) {
				//����
				CM->mCardsEnum = CardsEnum::AA;
			}
			else {
				return false;
			}
			break;
		case 3:
			if (arr[0] % 13 == arr[1] % 13 && arr[0] % 13 == arr[2] % 13) {
				//����
				CM->mCardsEnum = CardsEnum::AAA;
			}
			else {
				return false;
			}
		case 4:
			if (arr[1] % 13 == arr[2] % 13) {
				if (arr[0] % 13 == arr[1] && arr[3] % 13 == arr[1] % 13) {
					//ը��
					CM->mCardsEnum = CardsEnum::AAAA;
				}
				else if (arr[0] % 13 == arr[1] % 13 || arr[3] % 13 == arr[1] % 13) {
					//����һ
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
		//���ֵĵ���,14���ƣ�Ԫ��Ϊ�������ִ���
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

		//�ȼ���ǲ��ǵ�˳
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
	







	//�Ƿ�Ϊ�غ��״γ���

	//�������Ƿ����ǰһ����

	//���ƴ��
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
