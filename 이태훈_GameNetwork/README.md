# 실행 안내

## Unity Editor·패키지·.NET SDK 버전
### 유니티 사용한 패키지
* NetCode For GameObject
* UniTask
* Multiplayer Play Mode
### 윈도우 .Net SDK 버전
* 10.0.402

## 시작할 씬과 콘솔 프로젝트 위치
### 시작할 씬
* CoinGameScene
### 콘솔 프로젝트 위치
* . . /GameNetworkProject/이태훈_GameNetwork/UdpPeer

## 유니티 실행 방법, UDP Peer 실행 방법
### 유니티 실행 방법
<summary>
1. Scenes폴더에 있는 CoinGameScene 씬에 들어갑니다.<br>
2. 유니티 에디터 맨 위 Window 탭 -> Multiplayer -> Multiplayer Play Mode를 누릅니다.<br>
3. 아래 이미지처럼 나오게 되고 Player 2를 활성화합니다.<br>
4. 활성화가 완료되면 게임 플레이를 눌러 테스트를 진행합니다.
<br>
<br>
<img src = "Evidence/Multiplayer Play Mode 화면_01.png" width = "600" >
</summary>

### UDP Peer 실행 방법
<summary>
1. Visual Studio Code를 실행합니다.<br>
2. GameNetworkProject/이태훈_GameNetwork 경로에 있는 UdpPeer폴더를 선택하여 엽니다.
<br>
3. 터미널을 열어서 <b>dotnet run <내 포트> <상대 포트> <이름></b> 을 입력하여 실행합니다. (<, > 기호는 제외입니다.)
</summary>

## 접속 주소·포트와 설정을 바꾸는 위치
* 유니티 같은 경우 Unity Transport 컴포넌트에 위치에 있고 Udp Peer 콘솔 프로젝트는 직접 실행하여 포트를 직접 설정하며 주소는 이미 로컬로 되어있습니다.

## 이동·코인 획득·시험 기능의 조작 방법
<summary>
1. 가장 먼저 <b>Connect Host 버튼</b>을 눌러서 서버를 엽니다.<br>
2. <b>다른 Game 창</b>에서는 <b>Connect Client 버튼</b>을 눌러서 접속합니다.<br>
3. <b>WASD</b> 키로 캐릭터를 움직여 코인을 획득하는지 테스트하고 <b>Disconnect Client 버튼</b>을 누른 후 다시 <b>Connect Client 버튼</b>을 눌러 데이터가 유지되는지 테스트합니다. (Host 게임 창에서는 당연히 불가능합니다.)<br>
</summary>

## 정상 실행 여부를 판단할 화면 또는 출력 내용
* 게임을 플레이하면 Online Status 텍스트로 접속 상태를 확인할 수 있습니다.

## 알려진 오류·미완성 기능·환경 제한

### 알려진 오류
* 현재 테스트 환경에서 확인된 알려진 오류 없습니다.

### 미완성 기능
* 현재 요구사항에 포함된 기능은 모두 구현을 하였습니다.

    (지금은 빠른 구현과 입력 반응속도를 위해 플레이어 
    NetworkTrasnform을 Owner 권한으로 설정을 해두었는데 이러면 핵 때문에 이동이 된 후에 서버에서 검증을 해줘야하는 번거러움이 있고 보안적으로도 좋지 않다는 것을 알게되었습니다.)

### 환경제한
* Windows 10/11 64비트 환경 기준으로 개발하였습니다.
* 모바일 및 WebGL 환경에서는 동작을 검증하지 않았습니다.




