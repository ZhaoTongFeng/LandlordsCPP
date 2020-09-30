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

	//2 3 4 5 6 7 8 9 10 j q k a jocker
	//�����������ֵ�����
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

	//��С�����±�
	int startIndex = 0;

	for (int i = 0; i < 14; i++) {
		if (startIndex == 0 && pointArr[i] != 0) {
			startIndex = i;
		}
		//0-4���Ƴ��ֵĴ���
		CM->A[pointArr[i]]++;
	}


	//���ͼ��
	if (count < 5) {
		switch (count)
		{
		case 1:
			CM->mCardsEnum = CardsEnum::A;
			break;
		case 2:
			if (pointArr[13]==2) {
				//��ը
				CM->mCardsEnum = CardsEnum::JJ;
			}
			else if (CM->A[2]==1) {
				//����
				CM->mCardsEnum = CardsEnum::AA;
			}
			else {
				return false;
			}
			break;
		case 3:
			if (CM->A[3] == 1) {
				//����
				CM->mCardsEnum = CardsEnum::AAA;
			}
			else {
				return false;
			}
		case 4:
			if (CM->A[4] == 1) {
				//ը��
				CM->mCardsEnum = CardsEnum::AAAA;
			}
			else if (CM->A[3] == 1 && CM->A[1] == 1) {
				//����һ
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
			//��˳��
			//����С������ʼ���������Ƶĵ�������������Ϊ0����˵����������������


			//����˳�Ӷ�����������startIndex��ȡֵ��Χ����ͬ
			//��������ͣ�  J  Q  K  A JK
			//pointArr�±꣺9 10 11 12 13
			//i = startIndex = 9
			//i < startIndex+count = 14
			//����startIndex+count ����С�� 14
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
			//˫˳��
			//�����浥˳������
			//������Χ����

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
			//��˳��
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
					//������
					//�������Ƕ���
					CM->mCardsEnum = CardsEnum::AAABB;
				}
				else if (CM->A[4] == 1 && CM->A[1] == 1) {
					//�Ĵ�һ
					CM->mCardsEnum = CardsEnum::AAAAB;
				}
			}
			else if(count==6&& pointArr[13] != 2){
				if ((CM->A[4] == 1 && CM->A[2] == 1)||(CM->A[4] == 1 && CM->A[1] == 2)) {
					//�Ĵ���
					CM->mCardsEnum = CardsEnum::AAABB;
				}
			}
			else if(CM->A[3]>=2&& count - CM->A[3] * 3 <= CM->A[3]&& pointArr[13] != 2){
				//�ɻ�	
				CM->mCardsEnum = CardsEnum::AAABBBCD;
			}
			else {
				return false;
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
