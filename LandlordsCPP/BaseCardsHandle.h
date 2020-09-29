#pragma once

//���ƻ�������
class BaseCardsHandle{
public:
	BaseCardsHandle(const int maxCount);
	virtual ~BaseCardsHandle();

	//ϴ��
	virtual void Shuffle() = 0;

	//��һ�����Ʒ�һ����
	void HandCard(class BaseCards* c);
	//��һ���ƶ������һ��Ԫ�ط�һ�ŵ���
	void HandCard(int& cardsEle);

	virtual void ComputeAttribute(){}

	//�ƶ��Ƿ�Ϊ��
	bool isEmpty() { return cards_ptr == -1; }

public:
	//��ȡ�ƶ�
	const int* GetCards() { return cards; }

	//��ǰ�ƶ�ʣ��
	int GetCardsCount() { return cards_ptr + 1; }

protected:

	//�ƶ�
	int* cards;

	//�ƶ�ջ��ָ��
	int cards_ptr;

	int MAX_CARDS_COUNT;
};

