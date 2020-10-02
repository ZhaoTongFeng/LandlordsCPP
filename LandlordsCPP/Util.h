#pragma once
#include <iostream>
#include <time.h>
#include <random>
#include <string>

using namespace std;
//��׼��һ��54���Ƶ��˿�
//ȡֵ��Χ��1-54

//15���ƣ���С���ֿ���
#ifndef RANK_COUNT
#define RANK_COUNT 15
#endif // !RANK_COUNT

#ifndef TIMES_COUNT
#define TIMES_COUNT 5
#endif // !TIMES_COUNT


class Util
{
public:
    //����ϴ�Ʒ�
    static void Shuffle(int* arr, const int& count);

    //��ȡ��������(15) 2 3 4 5 6 7 8 9 10 J Q K A LJ RJ
    static int GetCardRank(const int& n);

    //��ȡ��������
    static string GetCardName(const int n);

    //ͳ��ÿ������������
    static int* GetNums(int* arr, const int& count);


    //arr_times��0�� 1�� 2�� 3�� 4�ų��ֵĴ�����ע�⣺��Ϊ��С���ķֿ��ģ����Զ����ᱻ����2�ŵ��ƣ�
    //index_beg����һ����λ��
    static void GetNumsAndTimes(int* arr, const int& count,int* arr_nums,int* arr_times,int& index_beg);

    //���յ�������Ĭ�ϴ�С����
    static void SortRank(int* arr, const int& count, bool littleFirst = true);

    //������������Ĭ�ϴ�С����
    static void SortNum(int* arr, const int& count, bool littleFirst = true);


    //��ӡINT����ʵ����
    static void PrintSource(const int* arr, const int& count);
    static std::string GetPrintSource(const int* arr, const int& count);

    //��ӡ����
    static void PrintName(const int* arr, const int& count);
    static std::string GetPrintName(const int* arr, const int& count);

    //���������������true
    static bool IsSequence(const int* arr_num, const int beg, const int end);
};

