import React, { useReducer, useState } from 'react';
import {
  SearchOutlined,
  PlusCircleFilled,
  InfoCircleFilled,
  QuestionCircleFilled,
  GithubFilled,
  LogoutOutlined
} from '@ant-design/icons';
import { theme, Input, Dropdown } from 'antd';

import { Outlet, useNavigate } from 'react-router-dom';
import logo from '@/logo.svg';
import { ProLayout, MenuDataItem, SettingDrawer, ProSettings, PageContainer } from '@ant-design/pro-components';
import { menus } from '@/config/menus';
import { CurrentUserState, logout } from '@/features/currentUser/currentUserSlice';
import { useAppDispatch, useAppSelector } from '@/app/hooks';

const loopMenuItem = (menus: any[]): MenuDataItem[] =>
  menus.map(({ title, key, children }) => ({
    name: title,
    path: key,
    children: children && loopMenuItem(children),
  }));
const Layout: React.FC = () => {
  const [settings, setSetting] = useState<Partial<ProSettings> | undefined>({
    layout: 'mix',
  });
  const {
    token: { colorBgContainer },
  } = theme.useToken();
  const navigate = useNavigate();
  const currentUser = useAppSelector(state=>state.currentUser.user);
  const dispatch = useAppDispatch();
  return (
    <>
      <ProLayout
        title={process.env.REACT_APP_NAME}
        logo={logo}
        location={{
          pathname: '/admin/process/edit/123',
        }}
        bgLayoutImgList={[
          {
            src: 'https://img.alicdn.com/imgextra/i2/O1CN01O4etvp1DvpFLKfuWq_!!6000000000279-2-tps-609-606.png',
            left: 85,
            bottom: 100,
            height: '303px',
          },
          {
            src: 'https://img.alicdn.com/imgextra/i2/O1CN01O4etvp1DvpFLKfuWq_!!6000000000279-2-tps-609-606.png',
            bottom: -68,
            right: -45,
            height: '303px',
          },
          {
            src: 'https://img.alicdn.com/imgextra/i3/O1CN018NxReL1shX85Yz6Cx_!!6000000005798-2-tps-884-496.png',
            bottom: 0,
            left: 0,
            width: '331px',
          },
        ]}
        // layout="mix"
        ErrorBoundary={false}
        avatarProps={{
          src: 'https://gw.alipayobjects.com/zos/antfincdn/efFD%24IOql2/weixintupian_20170331104822.jpg',
          size: 'small',
          title: currentUser.name,
          render: (props, dom) => {
            return (
              <Dropdown
                menu={{
                  onClick: props => {
                    if (props.key === 'logout') {
                      dispatch(logout());
                      navigate('login');
                    }
                  },
                  items: [
                    {
                      key: 'logout',
                      icon: <LogoutOutlined />,
                      label: '退出登录',
                    },
                  ],
                }}
              >
                {dom}
              </Dropdown>
            );
          }
        }}
        actionsRender={(props) => {
          if (props.isMobile) return [];
          return [
            props.layout !== 'side' ? (
              <div
                key="SearchOutlined"
                aria-hidden
                style={{
                  display: 'flex',
                  alignItems: 'center',
                  marginInlineEnd: 24,
                }}
                onMouseDown={(e) => {
                  e.stopPropagation();
                  e.preventDefault();
                }}
              >
                <Input
                  style={{
                    borderRadius: 4,
                    marginInlineEnd: 12,
                    backgroundColor: 'rgba(0,0,0,0.03)',
                  }}
                  prefix={
                    <SearchOutlined
                      style={{
                        color: 'rgba(0, 0, 0, 0.15)',
                      }}
                    />
                  }
                  placeholder="搜索方案"
                  bordered={false}
                />
                <PlusCircleFilled
                  style={{
                    color: 'var(--ant-primary-color)',
                    fontSize: 24,
                  }}
                />
              </div>
            ) : undefined,
            // <InfoCircleFilled key="InfoCircleFilled" />,
            // <QuestionCircleFilled key="QuestionCircleFilled" />,
            // <GithubFilled key="GithubFilled" />,
          ];
        }}
        // route={{routes:routes}}
        menu={{ request: async () => loopMenuItem(menus) }}
        menuItemRender={(item: MenuDataItem, dom) => {
          return (
            <div
              onClick={() => {
                if (!item.path) {
                  console.error(`menu route [${item.path}]:not found`);
                  return;
                }
                navigate(item.path);
              }}
            >
              {dom}
            </div>
          )
        }}
        menuFooterRender={(props) => {
          if (props?.collapsed) return undefined;
          return (
            <div
              style={{
                textAlign: 'center',
                paddingBlockStart: 12,
              }}
            >
              <div>© 2023.8 Powered</div>
              <div>by Ant Design + .NET Core</div>
            </div>
          );
        }}
        {...settings}
      >
        <Outlet />
      </ProLayout>
      <SettingDrawer
        // pathname={pathname}
        enableDarkTheme
        getContainer={() => document.getElementById('test-pro-layout')}
        settings={settings}
        onSettingChange={(changeSetting) => {
          setSetting(changeSetting);
        }}
        disableUrlParams={false}
      />
    </>
  )
};
export default Layout;
