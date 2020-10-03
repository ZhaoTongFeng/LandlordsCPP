#pragma once
#include <string>
using namespace std;

//�ƶ�ջ
class CardsBuffer
{
public:
	CardsBuffer(const int size);
	//�������캯�������moveΪ�棬���ڸ��ƹ���֮�����target
	CardsBuffer(CardsBuffer* target, bool move = false);
	virtual ~CardsBuffer() { delete[] buf; }

	//��ջ�������
	bool Push(const int& x);
	
	//��ջ���ƶ���
	bool Pop(int& x);
	bool Pop(CardsBuffer* target);
	bool Pop(CardsBuffer* target, int n);
	bool Get(int& x);

	//״̬
	bool IsEmpty()const { return top == -1; }
	bool IsFull()const { return top == maxSize - 1; }
	virtual void MakeEmpty() { top = -1; }


	//����״̬
	virtual void ComputeAttributes() {};

	//��ȡ��������
	virtual string GetCardsModeName() { return ""; };

	//���㵱ǰ������
	virtual void ComputeCardsMode() {};

	//�Ƚ���������,���A��B�󷵻�true
	virtual bool Compare(CardsBuffer* A, CardsBuffer* B) { return true; };

	//ϴ��
	void Shuffle();

	//���յ�������Ĭ�ϴ�С����
	void SortRank(bool littleFirst = true);

	//������������Ĭ�ϴ�С����
	void SortNum(bool littleFirst = true);

	//ͳ��ÿ������������
	void GetNums(int* arr_nums);

	void GetNumsAndTimes(int* arr_nums, int* arr_times, int& index_beg);


	//Getter/Setter
	int* GetBuf() { return buf; }
	int GetSize()const { return top + 1; }
	int GetMaxSize()const { return maxSize; }
	void SetMax() { top = maxSize - 1; }

	//INT����ʵ����
	std::string GetPrintSource();
	//����
	std::string GetPrintName();

protected:
	int* buf;
	int top, maxSize;
};

