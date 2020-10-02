#include "Cards.h"
#include "Util.h"

//��˳��
//����С������ʼ���������Ƶĵ�������������Ϊ0����˵����������������
//����˳�Ӷ�����������startIndex��ȡֵ��Χ����ͬ
//��������ͣ�  J  Q  K  A LJ RJ
//arr_nums�±꣺9 10 11  12 13 14
//i = startIndex = 9
//i < startIndex+size = 14 
//����startIndex+size ����С�� 14

//˫˳��
//�����浥˳������
//������Χ����

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

	//����ģʽ
	mCardsEnum = CardsEnum::ERR;

	//�����������ֵ�����
	int* arr_nums = nullptr;
	//0�� 1�� 2�� 3�� 4�ų��ֵĴ�����ע�⣺��Ϊ��С���ķֿ��ģ����Զ����ᱻ����2�ŵ��ƣ�
	int* arr_times = nullptr;
	//��С�����±�
	int startIndex = 0;
	
	//�Դ�����ư���������������
	//��ʱ��ͬ������С�����������
	if (GetSize() > 1) {
		SortNum(true);
	}
	GetNumsAndTimes(arr_nums, arr_times, startIndex);
	
	if (arr_nums == nullptr || arr_times == nullptr) {
		return;
	}

	//2.���ͼ��
	if (size < 5) {
		switch (size)
		{
		case 1:
			mCardsEnum = CardsEnum::A;
			break;
		case 2:
			if (arr_nums[13] == 1 && arr_nums[14] == 1) { mCardsEnum = CardsEnum::JJ; }//��ը
			else if (arr_times[2] == 1) { mCardsEnum = CardsEnum::AA; }//����
			break;
		case 3:
			if (arr_times[3] == 1) { mCardsEnum = CardsEnum::AAA; }//����
			break;
		case 4:
			if (arr_times[4] == 1) { mCardsEnum = CardsEnum::AAAA; }//ը��
			else if (arr_times[3] == 1 && arr_times[1] == 1) { mCardsEnum = CardsEnum::AAAB; }//����һ
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
				mCardsEnum = CardsEnum::ABCDE;//��˳��
			}
		}
		else if (size % 2 == 0 && arr_times[2] == double_seq_count) {
			SortRank(true);
			if (startIndex > 0 && startIndex + double_seq_count <= 13 && Util::IsSequence(arr_nums, startIndex, startIndex + double_seq_count)) {
				mCardsEnum = CardsEnum::AABBCC;//˫˳��
			}
		}
		else if (size % 3 == 0 && arr_times[3] == third_seq_count) {
			SortRank(true);
			if (startIndex > 0 && startIndex + third_seq_count <= 13 && Util::IsSequence(arr_nums, startIndex, startIndex + third_seq_count)) {
				mCardsEnum = CardsEnum::AAABBB;//��˳��
			}
		}
		else {
			if (size == 5) {
				if (arr_times[3] == 1 && arr_times[2] == 1) { mCardsEnum = CardsEnum::AAABB; }//������
				else if (arr_times[4] == 1 && arr_times[1] == 1) { mCardsEnum = CardsEnum::AAAAB; }//�Ĵ�һ
			}
			else if (size == 6) {
				if ((arr_times[4] == 1 && arr_times[2] == 1)) { mCardsEnum = CardsEnum::AAABB; }//�Ĵ���
				else if (arr_times[4] == 1 && arr_times[1] == 2 && !(arr_nums[14] == 1 && arr_nums[13] == 1)) {
					mCardsEnum = CardsEnum::AAABC;//�Ĵ����ŵ���
				}
			}
			else if (arr_times[3] >= 2 && size - arr_times[3] * 3 <= arr_times[3] 
				&& !(arr_nums[14] == 1 && arr_nums[13] == 1)) {
				mCardsEnum = CardsEnum::AAABBBCD;//�ɻ�	
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
	
	//��ը
	if (modeA == CardsEnum::JJ) {
		return true;
	}
	if (modeB == CardsEnum::JJ) {
		return false;
	}
	
	//ը��
	if (modeA == CardsEnum::AAAA && modeB != CardsEnum::AAAA) {
		return true;
	}
	if (modeA != CardsEnum::AAAA && modeB == CardsEnum::AAAA) {
		return false;
	}
	
	//���Ͳ�ͬ
	if (modeA != modeB) {
		return false;
	}

	//��ͬ���͵���������һ����
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
	//��ǰλ����Player������
	int count_pre = cards_pre->GetSize();

	if (count_pre == 0) {
		return false;
	}
	//������ȷ������
	cards_pre->ComputeCardsMode();
	if (cards_pre->mCardsEnum == CardsEnum::ERR) {
		return false;
	}
	//�������������
	if (cards_last != nullptr && !Compare(cards_pre, cards_last)) {
		return false;
	}

	//�������������ƴ��
	//����֮����Ҫ���ƣ�ֻ��Ҫ��û�б�������ư���ԭ����˳����ӵ��µ�������
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
		return "����";
	case CardsEnum::JJ:
		return "��ը";
	case CardsEnum::AA:
		return "����";
	case CardsEnum::AAA:
		return "����";
	case CardsEnum::AAAA:
		return "ը��";
	case CardsEnum::AAAB:
		return "����һ";
	case CardsEnum::ABCDE:
		return "˳��";
	case CardsEnum::AAABB:
		return "������";
	case CardsEnum::AAAAB:
		return "�Ĵ�һ";
	case CardsEnum::AAAABC:
		return "�Ĵ���";
	case CardsEnum::AABBCC:
		return "����";
	case CardsEnum::AAABBB:
		return "��˳��";
	case CardsEnum::AAABBBCD:
		return "�ɻ�";
	case CardsEnum::ERR:
		return "����";
	default:
		return "û���ҵ�";
		break;
	}
}