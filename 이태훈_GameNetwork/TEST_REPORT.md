
# 시험 기록

## NGO

<table>
  <thead>
    <tr>
      <th width="23%" align="left">항목</th>
      <th width="77%" align="left">내용</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><b>시험 대상</b></td>
      <td>NGO</td>
    </tr>
    <tr>
      <td><b>실행 조건·재현 순서</b></td>
      <td>
        <ol>
          <li>(다른 게임 창)Host Connect Button 클릭 후 다른 버튼도 클릭, 작동안되는지 확인</li>
          <li>Client Connect Button 클릭 후 Host Connect Button 클릭, 작동안되는지 확인</li>
          <li>플레이어 WASD 이동(서버랑 붙어있기)</li>
          <li>씬창에서 코인 Transform Pos를 두 플레이어 근처로 강제로 먹이게 하기(한쪽만 스코어 증가되는지 체크)</li>
          <li>Disconnect Client Button와 Connect Client Button 클릭 빠르게 반복</li>
          <li>Connect Client Button 누른 타이밍에 Online Status 체크</li>
          <li>Score 유지 체크</li>
          <li>플레이어 중복 생성되는지 체크</li>
        </ol>
      </td>
    </tr>
    <tr>
      <td><b>기대 결과</b></td>
      <td>양호</td>
    </tr>
    <tr>
      <td><b>실제 결과·측정값</b></td>
      <td>접속 후 Online Status: Online으로 표시되며 Score도 양호하며 플레이어 중복생성되지 않고 에러로그 없음</td>
    </tr>
    <tr>
      <td><b>수정·조정 내용</b></td>
      <td>없음(버튼을 또 눌러도 문제없으며 콘솔에 기본 로그가 나오도록 하였음)</td>
    </tr>
    <tr>
      <td><b>다시 확인한 결과</b></td>
      <td>양호</td>
    </tr>
  </tbody>
</table>

## UDP

<table>
  <thead>
    <tr>
      <th width="23%" align="left">항목</th>
      <th width="77%" align="left">내용</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><b>시험 대상</b></td>
      <td>UDP</td>
    </tr>
    <tr>
      <td><b>실행 조건·재현 순서</b></td>
      <td>
        <ol>
          <li>두 커미널에 dotnet run &lt;내 포트&gt; &lt;상대 포트&gt; &lt;이름&gt; 입력(한쪽은 이름을 전송량 제한값보다 클 정도로 입력)</li>
          <li>전송량 조건 비교가 에러없이 잘 되는지 확인</li>
          <li>이름 다시 입력(정상적으로)</li>
          <li>0(전송모드) 선택</li>
          <li>변경할 위치 입력 및 전송</li>
          <li>상대 터미널에서 받았는지 확인</li>
          <li>상대 터미널도 이와 같은 방식으로 위치 전송 및 확인</li>
          <li>같은 위치 입력</li>
          <li>무시되었는지 로그 확인</li>
          <li>1(무시된 메시지 보기) 입력</li>
          <li>무시된 메시지인지 확인</li>
          <li>0(뒤로가기) 입력</li>
          <li>0(전송모드) 입력</li>
          <li>숫자아닌 문자나 기호 입력해서 다시시도되는지 확인</li>
          <li>3(나가기) 입력</li>
          <li>에러없이 나가지는지 체크</li>
        </ol>
      </td>
    </tr>
    <tr>
      <td><b>기대 결과</b></td>
      <td>문제 1(전송량 조건 비교를 넣지 않으면 이름 비정상적으로 길게 지을 때 에러가 발생하거나 데이터가 잘려나가는 문제가 생긴다는 것을 알았음)</td>
    </tr>
    <tr>
      <td><b>실제 결과·측정값</b></td>
      <td>위치 송수신 성공적으로 잘되며 같은 위치를 입력하였을 때 수신되지 않고 무시된 메시지로 기록되는 걸 확인하였음.</td>
    </tr>
    <tr>
      <td><b>수정·조정 내용</b></td>
      <td>이름같은 경우 string 가변 데이터이기 때문에 전송량 제한걸어서 안정적이게 송수신하도록 수정하였음 (위치 데이터나 순서번호는 int 4바이트 고정이기 때문에 제한에 굳이 포함하지않았고 이름을 만들고 시작하기 때문에 처음부터 조건을 걸었음)</td>
    </tr>
    <tr>
      <td><b>다시 확인한 결과</b></td>
      <td>양호</td>
    </tr>
  </tbody>
</table>

* UDP 프로그램에서 이름 크기 제한 전후 결과 비교는 위 테이블에 적혀있습니다. (기대 결과와 수정·조정 내용)
