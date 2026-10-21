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
<img src = "Evidence/Multiplayer Play Mode 화면_01.PNG" width = "600" >
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
* 가장 먼저 Connect Host 버튼을 눌러서 서버를 엽니다.
* 다른 Game 창에서는 Connect Client 버튼을 눌러서 접속합니다.
* WASD 키로 캐릭터를 움직여 코인을 획득하는지 테스트하고 Disconnect Client 버튼을 누른 후 다시 Connect Client를 눌러 데이터가 유지되는지 테스트합니다. (Host 게임 창에서는 당연히 불가능합니다.)

## 정상 실행 여부를 판단할 화면 또는 출력 내용





