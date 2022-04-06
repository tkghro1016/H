1. 명명 규칙

1) _없이 대문자 소문자
	Ex) BtnCloseClicked

2) 각 타입 별 명명법
	String		str
	Int			int
	Float			flt
	메소드		Fnc
	클래스		Cls
	창				Win

3) 각 컴포넌트 별 명명법
	버튼			Btn
	라벨			Lbl
	이미지		Img
	텍스트		Txt (텍스트 박스)
	체크박스	Chk
	그리드		Grd
	콤보박스	Cbo
	슬라이더		Sld
	라디오버튼  Rad

3) WPF 관리방법
	Main Window는 가장 손이 많이 가고 로직이 많이 들어가야 하므로 Root에 저장
	SubWindow들은 subWindows에 저장
	popUps들은 메세지 창 등의 sub창이 아닌 트랜잭션처리를 위한 팝업창을 저장
	윈도우의 기본 최소화, 최대화, 닫기 기능을 사용하지 않기 때문에 각각을 버튼으로 만들어서 제공해야함

2. 참고 사항

1) WPF는 Windows간의 형태 상속을 지원하지 않는다.
   - 편법적인 방법이 있기는 하지만 안하는 것이 나을 것 같다.  괜히 꼬일거 같음
   - 사실 그래서 subWindow들을 한번에 구축하는데에 실패하였다.
   - 하지만 최대한 겹치는 부분은 맞춰서 하도록 

2) 살려줘


