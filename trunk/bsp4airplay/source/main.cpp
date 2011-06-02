#include <s3e.h>
#include <s3ePointer.h>
#include <s3eKeyboard.h>
#include <IwGx.h>
#include <IwGraphics.h>
#include <IwAnim.h>
#include <Bsp4Airplay.h>
#include <b4aPlane.h>
#include <Ib4aProjection.h>

using namespace Bsp4Airplay;

iwangle angleZ = 0;
iwangle angleBow = 0;
int32 oldMouseX = 0;
int32 oldMouseY = 0;
int32 isButtonDown = 0;
bool moveForward = false;
bool moveBackward = false;
bool moveLeft = false;
bool moveRight = false;
bool invertMouse = false;
int32 pointerButton (s3ePointerEvent* systemData, void* userData)
{
	isButtonDown = (systemData->m_Pressed)?systemData->m_Button+1:0;
	oldMouseX = systemData->m_x;
	oldMouseY = systemData->m_y;
	return 0;
}
void RotateCamera(int dx, int dy)
{
	if (invertMouse)
	{
		dx = -dx;
		dy = -dy;
	}
	angleZ = (angleZ-dx) & 4095;
	angleBow = angleBow+dy;
	if (angleBow < -1000) angleBow = -1000;
	if (angleBow > 1000) angleBow = 1000;
}

int32 pointerMotion (s3ePointerMotionEvent* systemData, void* userData)
{
	if (isButtonDown)
	{
		int32 dx = (systemData->m_x-oldMouseX)*4096/(int32)IwGxGetScreenWidth(); oldMouseX=systemData->m_x;
		int32 dy = (systemData->m_y-oldMouseY)*4096/(int32)IwGxGetScreenHeight(); oldMouseY=systemData->m_y;
		RotateCamera(dx,dy);
	}
	return 0;
}
int32 pointerTouchMotion (s3ePointerTouchMotionEvent* systemData, void* userData)
{
	int32 dx = (systemData->m_x-oldMouseX)*4096/(int32)IwGxGetScreenWidth(); oldMouseX=systemData->m_x;
	int32 dy = (systemData->m_y-oldMouseY)*4096/(int32)IwGxGetScreenHeight(); oldMouseY=systemData->m_y;
	RotateCamera(dx,dy);

	return 0;
}
CIwTexture* flashlight_tex = 0;
Cb4aFlashlightProjection* flashlight=0;
int32 keyboardEvent (s3eKeyboardEvent* systemData, void* userData)
{
	switch (systemData->m_Key)
	{
	case s3eKeyW:
	case s3eKeyUp:
		moveForward = 0 != systemData->m_Pressed;
		break;
	case s3eKeyS:
	case s3eKeyDown:
		moveBackward = 0 != systemData->m_Pressed;
		break;
	case s3eKeyA:
	case s3eKeyLeft:
		moveLeft = 0 != systemData->m_Pressed;
		break;
	case s3eKeyD:
	case s3eKeyRight:
		moveRight = 0 != systemData->m_Pressed;
		break;
	case s3eKeyL:
		if (flashlight == 0)
			flashlight = new Cb4aFlashlightProjection();
		flashlight->Prepare(flashlight_tex, IwGxGetViewMatrix(),CIwVec3(256,256,512));
		break;
	default:
		return 0;
	}
	return 0;
}
inline CIwSVec3 World2Level(const CIwVec3 & w)
{
	return CIwSVec3((int16)(w.x>>IW_GEOM_POINT),(int16)(w.y>>IW_GEOM_POINT),(int16)(w.z>>IW_GEOM_POINT));
}
inline CIwVec3 Level2World(const CIwSVec3 & w, const CIwVec3 & fractionPart)
{
	return CIwVec3(((int32)w.x)<<IW_GEOM_POINT,((int32)w.y)<<IW_GEOM_POINT,((int32)w.z)<<IW_GEOM_POINT);
}
void RenderFlares(Cb4aLevel* level,int16 size)
{
	CIwVec3 rawRight = IwGxGetViewMatrix().RowX()*size;
	CIwVec3 rawUp = IwGxGetViewMatrix().RowY()*size;
	for (uint32 i=0; i<level->GetNumEntities(); ++i)
	{
		const Cb4aEntity* e = level->GetEntityAt(i);
		if (!e) continue;
		if (e->GetClassName() == "light")
		{
			CIwSVec3 o(e->GetOrigin());
			if (!level->IsInVisibleArea(e->GetOrigin()))
				continue;
			CIwSVec3* p = IW_GX_ALLOC(CIwSVec3,4);
			CIwSVec2* uv = IW_GX_ALLOC(CIwSVec2,4);
			p[0] = o-rawRight-rawUp;
			uv[0] = CIwSVec2(0,0);
			p[1] = o-rawRight+rawUp;
			uv[1] = CIwSVec2(0,IW_GEOM_ONE);
			p[2] = o+rawRight+rawUp;
			uv[2] = CIwSVec2(IW_GEOM_ONE,IW_GEOM_ONE);
			p[3] = o+rawRight-rawUp;
			uv[3] = CIwSVec2(IW_GEOM_ONE,0);
			IwGxSetVertStreamWorldSpace(p,4);
			IwGxSetColStream(0,0);
			IwGxSetNormStream(0,0);
			IwGxSetUVStream(uv,0);
			IwGxSetUVStream(0,1);
			IwGxDrawPrims(IW_GX_QUAD_LIST,0,4);
			IwGxSetUVStream(0,0);
		}
	}
}
//-----------------------------------------------------------------------------
// Main global function
//-----------------------------------------------------------------------------
int main(int argc, char* argv[])
{
	IwGxInit();
	IwResManagerInit();
	IwGraphicsInit();
	IwAnimInit();
	Bsp4Airplay::Bsp4AirpayInit();
	s3eKeyboardRegister(S3E_KEYBOARD_KEY_EVENT, (s3eCallback)keyboardEvent, 0);

	s3ePointerRegister(S3E_POINTER_BUTTON_EVENT, (s3eCallback)pointerButton, 0);
	s3ePointerRegister(S3E_POINTER_MOTION_EVENT, (s3eCallback)pointerMotion, 0);
	s3ePointerRegister(S3E_POINTER_TOUCH_MOTION_EVENT, (s3eCallback)pointerTouchMotion, 0);

	IwGxSetColClear(0x7f, 0x7f, 0x7f, 0x7f);
	IwGxPrintSetColour(128, 128, 128);

	CIwResGroup* fx_group = IwGetResManager()->LoadGroup("./fx.group");
	flashlight_tex = (CIwTexture*)fx_group->GetResNamed("flashlight","CIwTexture");
	flashlight_tex->SetClamping(true);
	CIwTexture* flare_tex = (CIwTexture*)fx_group->GetResNamed("flare","CIwTexture");

	CIwResGroup* npc_group = IwGetResManager()->LoadGroup("./models/guerilla.group");
	CIwModel* npc_model = (CIwModel*)npc_group->GetResNamed("guerilla","CIwModel");
	CIwAnimSkin* npc_skin = (CIwAnimSkin*)npc_group->GetResNamed("guerilla","CIwAnimSkin");
	CIwAnimSkel* npc_skel = (CIwAnimSkel*)npc_group->GetResNamed("guerilla","CIwAnimSkel");
	CIwAnimPlayer* npc_player = new CIwAnimPlayer;
	npc_player->SetSkel(npc_skel);

	CIwResGroup* model_group = IwGetResManager()->LoadGroup("./models/v_m4a1.group");
	CIwModel* hands_model = (CIwModel*)fx_group->GetResNamed("v_m4a1","CIwModel");
	CIwAnimSkin* hands_skin = (CIwAnimSkin*)npc_group->GetResNamed("v_m4a1","CIwAnimSkin");
	CIwAnimSkel* hands_skel = (CIwAnimSkel*)npc_group->GetResNamed("v_m4a1","CIwAnimSkel");

	//CIwVec3 rawDown(0,0,-4096);
	CIwVec3 rawDown(0,0,0);

	//const char* defaultGroupName = "maps/samplebox.group";
	//const char* defaultGroupName = "maps/al_test_map_02.group";
	//const char* defaultGroupName = "maps/sg0503.group";
	const char* defaultGroupName = "maps/de_dust.group";
	//const char* defaultGroupName = "maps/qzdm1.group";
	//const char* defaultGroupName = "maps/q3shw18.group";
	//const char* defaultGroupName = "maps/cs_mansion.group";
	//const char* defaultGroupName = "maps/de_aztec.group";
	//const char* defaultGroupName = "maps/match1.group";
	const char* groupName = (argc > 1)?argv[1]:defaultGroupName;

	//CIwResGroup* group = IwGetResManager()->LoadGroup("maps/hldemo1.group");
	//Bsp4Airplay::Cb4aLevel* level = static_cast<Bsp4Airplay::Cb4aLevel*>(group->GetResNamed("hldemo1", "Cb4aLevel"));

	//CIwResGroup* group = IwGetResManager()->LoadGroup("maps/sg0503.group");
	//Bsp4Airplay::Cb4aLevel* level = static_cast<Bsp4Airplay::Cb4aLevel*>(group->GetResNamed("sg0503", "Cb4aLevel"));


	//CIwResGroup* group = IwGetResManager()->LoadGroup("maps/leonHL2_1.group");
	//Bsp4Airplay::Cb4aLevel* level = static_cast<Bsp4Airplay::Cb4aLevel*>(group->GetResNamed("leonHL2_1", "Cb4aLevel"));

	//CIwResGroup* group = IwGetResManager()->LoadGroup("maps/match1.group");
	//Bsp4Airplay::Cb4aLevel* level = static_cast<Bsp4Airplay::Cb4aLevel*>(group->GetResNamed("match1", "Cb4aLevel"));

	//CIwResGroup* group = IwGetResManager()->LoadGroup("maps/madcrabs.group");
	//Bsp4Airplay::Cb4aLevel* level = static_cast<Bsp4Airplay::Cb4aLevel*>(group->GetResNamed("madcrabs", "Cb4aLevel"));

	//CIwResGroup* group = IwGetResManager()->LoadGroup("maps/q3shw18.group");
	//Bsp4Airplay::Cb4aLevel* level = static_cast<Bsp4Airplay::Cb4aLevel*>(group->GetResNamed("q3shw18", "Cb4aLevel"));

	CIwResGroup* group = IwGetResManager()->LoadGroup(groupName);
	CIwResList* list = group->GetListNamed("Cb4aLevel");

	

	Bsp4Airplay::Cb4aLevel* level = 0;
	if (list && list->m_Resources.GetSize() > 0)
		level = (Bsp4Airplay::Cb4aLevel*)list->m_Resources[0];


	int spawnEntIndex = level->FindEntityByClassName("info_player_start");
	if (spawnEntIndex < 0)
		spawnEntIndex = level->FindEntityByClassName("info_player_deathmatch");
	else
	{
		int b = level->FindEntityByClassName("info_player_start",spawnEntIndex+1);
		if (b >= 0) spawnEntIndex = b;
	}
	const Bsp4Airplay::Cb4aEntity* spawnEnt = (spawnEntIndex>=0)?level->GetEntityAt(spawnEntIndex):0;
	CIwVec3 playerOrigin = spawnEnt ? (spawnEnt->GetOrigin()) : CIwVec3::g_Zero;
	CIwVec3 npcOrigin = playerOrigin;
	playerOrigin.x <<= IW_GEOM_POINT;
	playerOrigin.y <<= IW_GEOM_POINT;
	playerOrigin.z <<= IW_GEOM_POINT;
	{
		
		while (1)
		{
			s3eDeviceYield(0);
			s3eKeyboardUpdate();
			s3ePointerUpdate();

			bool result = true;
			if	(
				(result == false) ||
				(s3eKeyboardGetState(s3eKeyEsc) & S3E_KEY_STATE_DOWN) ||
				(s3eKeyboardGetState(s3eKeyAbsBSK) & S3E_KEY_STATE_DOWN) ||
				(s3eDeviceCheckQuitRequest())
				)
				break;

			IwGxClear(IW_GX_DEPTH_BUFFER_F);
			//IwGxClear(IW_GX_COLOUR_BUFFER_F | IW_GX_DEPTH_BUFFER_F);

			CIwMat view;
			CIwMat model;
			//if (spawnEnt >= 0)
			/*{
				const Bsp4Airplay::Cb4aEntity* e = level->GetEntityAt(spawnEnt);
				view.LookAt(CIwVec3(0,0,0),CIwVec3(0,IW_GEOM_ONE,0),CIwVec3(0,0,IW_GEOM_ONE));

				view.SetTrans(e->GetOrigin());
			}
			else
			{
				view.LookAt(CIwVec3(0,0,IW_GEOM_ONE),CIwVec3(0,0,0),CIwVec3(0,IW_GEOM_ONE,0));
				view.SetTrans(CIwVec3(512,512,1048));
			}*/
			
			view.LookAt(CIwVec3(0,0,0),CIwVec3(0,IW_GEOM_ONE,0),CIwVec3(0,IW_GEOM_ONE,0));
			view.PreRotateY(angleZ);
			view.PreRotateX(angleBow);
			view.SetTrans(CIwVec3::g_Zero);
			CIwVec3 rawForward = view.RowZ();
			CIwVec3 rawRight = view.RowX();
			CIwVec3 rawUp = -view.RowY();
			

			CIwVec3 forward = CIwVec3(rawForward.x*16,rawForward.y*16,rawForward.z*16);
			CIwVec3 right = CIwVec3(rawRight.x*16,rawRight.y*16,rawRight.z*16);
			CIwVec3 down = CIwVec3(rawDown.x*4,rawDown.y*4,rawDown.z*4);
			CIwVec3 playerMovement = CIwVec3::g_Zero;
			if (moveForward)
				playerMovement += forward;
			if (moveBackward)
				playerMovement -= forward;
			if (moveRight)
				playerMovement += right;
			if (moveLeft)
				playerMovement -= right;
			playerMovement += down;
			Bsp4Airplay::Cb4aTraceContext context;
			context.from = playerOrigin;
			context.to = playerOrigin+playerMovement;

			CIwVec3 originalMovement = playerMovement;
			playerOrigin += playerMovement;
			int count = 0;
			int32 playerRadius = 32<<IW_GEOM_POINT;
			while (context.from != context.to)
			{
				CIwVec3 originalDestination = context.to;
				if (!level->TraceSphere(playerRadius,context))
					break;
				CIwVec3 distToPlane = CIwVec3(abs(originalDestination.x-context.to.x),abs(originalDestination.y-context.to.y),abs(originalDestination.z-context.to.z));
				originalDestination.x += (context.collisionNormal.x*distToPlane.x)/IW_GEOM_ONE;
				originalDestination.y += (context.collisionNormal.y*distToPlane.y)/IW_GEOM_ONE;
				originalDestination.z += (context.collisionNormal.z*distToPlane.z)/IW_GEOM_ONE;
				while(playerRadius > Bsp4Airplay::b4aPlaneDist(originalDestination, Cb4aPlane(context.collisionNormal, context.collisionPlaneD)))
				{
					originalDestination+= context.collisionNormal;
				}

				context.from = context.to;
				context.to = originalDestination;
				/*if (originalMovement.x*(context.to.x-context.from.x) +
					originalMovement.y*(context.to.y-context.from.y)+
					originalMovement.z*(context.to.z-context.from.z) < 0)
				{
					playerOrigin = context.from;
					break;
				}*/

				++count;
				if (count > 3 || (abs(context.from.x-context.to.x) <= 64 && abs(context.from.y-context.to.y) <= 64 && abs(context.from.z-context.to.z) <= 64))
				{
					playerOrigin = context.from;
					break;
				}
				playerOrigin = context.to;
			}
			CIwSVec3 cameraOrigin = World2Level(playerOrigin);
			view.SetTrans(cameraOrigin);

			IwGxSetViewMatrix(&view);

			model.SetIdentity();
			IwGxSetModelMatrix(&model);

			int32 screenWidth2 =IwGxGetScreenWidth()/2;
			//int32 screenHeight2 =IwGxGetScreenHeight()/2;
			//CIwVec3 testV = view.TransposeTransformVec(view.t+view.RotateVec(CIwVec3(-screenWidth2,-screenHeight2,screenWidth2)));

			IwGxSetPerspMul(screenWidth2);

			//Actual distance will be calculated at level->BeginRender
			IwGxSetFarZNearZ(32767,8);

			

			level->BeginRender();
			if (flashlight)
			{
				flashlight->Clear();
				level->RenderProjection(flashlight);
			}
			level->EndRender();
			
			context.from = playerOrigin;
			context.to = context.from+CIwVec3(rawForward.x*400,rawForward.y*400,rawForward.z*400);
			if (level->TraceLine(context))
			{
				CIwMaterial* m= IW_GX_ALLOC_MATERIAL();
				m->SetZDepthOfs(-1);
				m->SetZDepthOfsHW(-1);
				IwGxSetMaterial(m);
				CIwSVec3* p = IW_GX_ALLOC(CIwSVec3,2*4);
				CIwColour* c = IW_GX_ALLOC(CIwColour,2*4);
				p[0] = World2Level(context.to);
				c[0].Set(255,255,255,255);
				p[1] = World2Level(context.to)+context.collisionNormal*20;
				c[1].Set(255,255,255,255);
				p[2] = World2Level(context.to)+CIwSVec3(10,0,0);
				c[2].Set(255,0,0,255);
				p[3] = World2Level(context.to)+CIwSVec3(-10,0,0);
				c[3].Set(255,0,0,255);
				p[4] = World2Level(context.to)+CIwSVec3(0,10,0);
				c[4].Set(0,255,0,255);
				p[5] = World2Level(context.to)+CIwSVec3(0,-10,0);
				c[5].Set(0,255,0,255);
				p[6] = World2Level(context.to)+CIwSVec3(0,0,10);
				c[6].Set(0,0,255,255);
				p[7] = World2Level(context.to)+CIwSVec3(0,0,-10);
				c[7].Set(0,0,255,255);
				IwGxSetVertStreamWorldSpace(p,2*4);
				IwGxSetColStream(c,2*4);
				IwGxSetNormStream(0,0);
				IwGxSetUVStream(0,0);
				IwGxSetUVStream(0,1);
				IwGxDrawPrims(IW_GX_LINE_LIST,0,2*4);
				IwGxSetColStream(0,0);
			}

			CIwMaterial* mat = IW_GX_ALLOC_MATERIAL();
			mat->SetTexture(flare_tex);
			mat->SetAlphaMode(CIwMaterial::ALPHA_ADD);
			mat->SetModulateMode(CIwMaterial::MODULATE_NONE);
			//mat->SetCullMode(CIwMaterial::CULL_NONE);
			mat->SetDepthWriteMode(CIwMaterial::DEPTH_WRITE_DISABLED);
			//IW_GX_SORT_BY_Z
			//mat->SetMergeGeom
			
			IwGxSetMaterial(mat);
			RenderFlares(level, 16);

			model.t = view.t// + CIwVec3(rawForward.x/2048,rawForward.y/2048,rawForward.z/2048)
				- CIwVec3(rawUp.x/150,rawUp.y/150,rawUp.z/150);
			model.CopyRot(view);
			model.PreRotateZ(IW_GEOM_ONE/2);
			model.PreRotateX(IW_GEOM_ONE/4);
			//model.ScaleRot(IW_GEOM_ONE/4); //8
			IwGxSetModelMatrix(&model);
			
			IwAnimSetSkelContext(hands_skel);
			IwAnimSetSkinContext(hands_skin);
			hands_model->Render();

			model.SetIdentity();
			model.t = npcOrigin;
			IwGxSetModelMatrix(&model);
			IwAnimSetSkelContext(npc_player->GetSkel());
			IwAnimSetSkinContext(npc_skin);
			npc_model->Render();
			IwAnimSetSkelContext(0);
			IwAnimSetSkinContext(0);

			IwGxFlush();
			IwGxSwapBuffers();
		}
		if (flashlight)
			delete flashlight;
	}
	delete npc_player;

	Bsp4Airplay::Bsp4AirpayTerminate();
	IwAnimTerminate();
	IwGraphicsTerminate();
	IwResManagerTerminate();
	IwGxTerminate();
	return 0;
}