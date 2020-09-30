#pragma once
#include "BaseGameMode.h"

enum class CallState
{
    NO,
    CALL,
    Rob
};

class GameMode :
    public BaseGameMode
{
public:
    GameMode();
    ~GameMode();
    void ProcessInput()override;
    void UpdateGame(float deltaTime)override;
    void GenerateOutput(std::string& str)override;
protected:
    void HandCards()override;
    void Settle()override;

    //����״̬
    CallState mCallState;
    //��¼������Ƶ�����
    int mCallArr[3]{ 0 };
    int mCallArrCount = 0;
    //��ǽе��������
    int mLandlordsIndex;

    //���ƴ�����С�ڵ����Ĵ�
    int numCall;

    //���Ƶ���
    int inputPoint;

    //����
    //��ѡ�е��Ʒŵ�һ���б��У�����Ƿ�������ͣ�����¼����
    bool gate = false;
    std::string mCardsBuffer;
    int mPreOutCards[20];
    int mPreOutCardsCount = 0;

    //һ�����ƻغϵĿ�ʼ���ܲ�����
    bool isStart = true;

    //���Ҳ�Ҫ�����ж�Ϊ�»غϿ�ʼ
    int mMissCount = 0;

};

