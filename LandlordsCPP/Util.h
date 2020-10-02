#pragma once
#include <iostream>
#include <time.h>
#include <random>
#include <string>

using namespace std;
//标准的一幅54张牌的扑克
//取值范围：1-54

//15种牌（大小王分开）
#ifndef RANK_COUNT
#define RANK_COUNT 15
#endif // !RANK_COUNT

#ifndef TIMES_COUNT
#define TIMES_COUNT 5
#endif // !TIMES_COUNT


class Util
{
public:
    //交叉洗牌法
    static void Shuffle(int* arr, const int& count);

    //获取点数排名(15) 2 3 4 5 6 7 8 9 10 J Q K A LJ RJ
    static int GetCardRank(const int& n);

    //获取单张牌名
    static string GetCardName(const int n);

    //统计每个点数的张数
    static int* GetNums(int* arr, const int& count);


    //arr_times：0张 1张 2张 3张 4张出现的次数（注意：因为大小王的分开的，所以对王会被当成2张单牌）
    //index_beg：第一张牌位置
    static void GetNumsAndTimes(int* arr, const int& count,int* arr_nums,int* arr_times,int& index_beg);

    //按照点数排序，默认从小到大
    static void SortRank(int* arr, const int& count, bool littleFirst = true);

    //按照张数排序，默认从小到大
    static void SortNum(int* arr, const int& count, bool littleFirst = true);


    //打印INT型真实点数
    static void PrintSource(const int* arr, const int& count);
    static std::string GetPrintSource(const int* arr, const int& count);

    //打印牌名
    static void PrintName(const int* arr, const int& count);
    static std::string GetPrintName(const int* arr, const int& count);

    //如果牌型连续返回true
    static bool IsSequence(const int* arr_num, const int beg, const int end);
};

