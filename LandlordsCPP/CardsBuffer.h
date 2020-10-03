#pragma once
#include <string>
using namespace std;

//牌堆栈
class CardsBuffer
{
public:
	CardsBuffer(const int size);
	//拷贝构造函数：如果move为真，则在复制过来之后清空target
	CardsBuffer(CardsBuffer* target, bool move = false);
	virtual ~CardsBuffer() { delete[] buf; }

	//入栈：添加牌
	bool Push(const int& x);
	
	//出栈：移动牌
	bool Pop(int& x);
	bool Pop(CardsBuffer* target);
	bool Pop(CardsBuffer* target, int n);
	bool Get(int& x);

	//状态
	bool IsEmpty()const { return top == -1; }
	bool IsFull()const { return top == maxSize - 1; }
	virtual void MakeEmpty() { top = -1; }


	//更新状态
	virtual void ComputeAttributes() {};

	//获取牌型名称
	virtual string GetCardsModeName() { return ""; };

	//计算当前的牌型
	virtual void ComputeCardsMode() {};

	//比较两个牌型,如果A比B大返回true
	virtual bool Compare(CardsBuffer* A, CardsBuffer* B) { return true; };

	//洗牌
	void Shuffle();

	//按照点数排序，默认从小到大
	void SortRank(bool littleFirst = true);

	//按照张数排序，默认从小到大
	void SortNum(bool littleFirst = true);

	//统计每个点数的张数
	void GetNums(int* arr_nums);

	void GetNumsAndTimes(int* arr_nums, int* arr_times, int& index_beg);


	//Getter/Setter
	int* GetBuf() { return buf; }
	int GetSize()const { return top + 1; }
	int GetMaxSize()const { return maxSize; }
	void SetMax() { top = maxSize - 1; }

	//INT型真实点数
	std::string GetPrintSource();
	//牌名
	std::string GetPrintName();

protected:
	int* buf;
	int top, maxSize;
};

