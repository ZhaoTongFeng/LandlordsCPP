#pragma once
#include "BaseGameMode.h"

//����״̬
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

    void ReStart()override;
    ////////////////////////////////////
    //����

    //���ŵ���
    class Cards* mDarkCards = nullptr;
    /////////////////////////////////////
    //����״̬
    CallState mCallState = CallState::NO;

    //������Ƶ����������±꣬��һ����Ϊ�е�������
    int mCallArr[3]{ 0 };
    int mCallArrCount = 0;

    //��ǽе��������
    int mLandlordsIndex = -1;

    //���ƴ�����С�ڵ����Ĵ�
    int numCall = 0;

    //���Ƶ���
    int inputPoint = -1;
    ////////////////////////////////////////
    //����
    //��ѡ�е��Ʒŵ�һ���б��У�����Ƿ�������ͣ�����¼����
    bool gate = false;

    //���Ҳ�Ҫ�����ж�Ϊ�»غϿ�ʼ
    int mMissCount = 0;

    //��һ����
    class Cards* mLastCards = nullptr;

    //׼��������
    //��ѡ����Ʒŵ����
    class Cards* mPreCards = nullptr;

};

