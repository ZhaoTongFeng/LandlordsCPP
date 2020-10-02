#include "SortCards.h"

int GetCardRank(const int& n) {
	if (n == 54) {
		return 14;
	}
	else if (n == 53) {
		return 13;
	}
	else if (n % 13 == 1) {
		return 12;
	}
	else if (n % 13 == 0) {
		return 11;
	}
	else {
		return n % 13 - 2;
	}
}

void SortPointL(int* arr, const int& count)
{
	int i, j, tmp, last, next;
	for (i = 0; i < count; i++) {
		for (j = count -1; j > i; j--) {
			last = GetCardRank(arr[j - 1]);
			next = GetCardRank(arr[j]);

			if (last > next) {
				tmp = arr[j - 1];
				arr[j - 1] = arr[j];
				arr[j] = tmp;
			}
		}
	}
}


void SortPointB(int* arr, const int& count)
{
	int i, j, tmp, last, next;
	for (i = 0; i < count; i++) {
		for (j = count - 1; j > i; j--) {
			last = GetCardRank(arr[j - 1]);
			next = GetCardRank(arr[j]);

			if (last < next) {
				tmp = arr[j - 1];
				arr[j - 1] = arr[j];
				arr[j] = tmp;
			}
		}
	}
}

void SortNumL(int* arr, const int& count)
{

}
#include <iostream>
using namespace std;

void SortNumB(int* arr, const int& count)
{
	//ÿ���˿˵�����
	int tmp_arr[15]{ 0 };
	for (int i = 0; i < count; i++) {
		tmp_arr[GetCardRank(arr[i])]++;
	}
	//�˿˵�˳��
	int tmp_index[15]{ 0 };
	for (int i = 0; i < 15; i++) {
		tmp_index[i] = i;
	}
	//�������ȸ��˿��������ȼ�
	int tmpa,tmpi;
	for (int i = 0; i < 15; i++) {
		for (int j = 15 - 1; j > i; j--) {
			if (tmp_arr[j - 1] < tmp_arr[j]) {

				tmpa = tmp_arr[j - 1];
				tmp_arr[j - 1] = tmp_arr[j];
				tmp_arr[j] = tmpa;

				tmpi = tmp_index[j - 1];
				tmp_index[j - 1] = tmp_index[j];
				tmp_index[j] = tmpi;
			}
		}
	}
	//�������˳��Գ��ƽ�������
	int tmp, tmp_a, tmp_b;
	for (int i = 0; i < count; i++) {
		for (int j = count-1; j > i; j--) {
			//�ҵ�����Ԫ�ض�Ӧ�����ȼ�,��ط�Ӧ�ÿ�����һ����ά������ά��������¼
			for (int k = 0; k < 15; k++) {
				if (GetCardRank(arr[j - 1]) == tmp_index[k]) {
					tmp_a = k;
				}
				if (GetCardRank(arr[j]) == tmp_index[k]) {
					tmp_b = k;
				}
			}
			if (tmp_a > tmp_b) {
				tmp = arr[j - 1];
				arr[j - 1] = arr[j];
				arr[j] = tmp;
			}
		}
	}
}
