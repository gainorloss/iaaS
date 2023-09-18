import React from 'react';
import ReactDOM from 'react-dom/client';

import reportWebVitals from './reportWebVitals';
import { Provider } from 'react-redux';
import { store } from './app/store';
import dayjs from 'dayjs';
import 'dayjs/locale/zh-cn';
import './index.css';
import { ConfigProvider } from 'antd';
import zhCN from 'antd/locale/zh_CN';
import { StyleProvider, legacyLogicalPropertiesTransformer, px2remTransformer } from '@ant-design/cssinjs';
import { App as AntdApp } from 'antd'
import App from './App';
dayjs.locale('zh-cn');
const px2rem = px2remTransformer({
  // rootValue: 19, // 32px = 1rem; @default 16
  mediaQuery: true
});


const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

root.render(
  // <React.StrictMode>
  <Provider store={store}>
    <ConfigProvider locale={zhCN}
      theme={{
        token: {
          // Seed Token，影响范围大
          // colorPrimary: '#00b96b',
          // borderRadius: 2,

          // 派生变量，影响范围小
          // colorBgContainer: '#f6ffed',
        },
      }}
    >
      <StyleProvider hashPriority="high" transformers={[legacyLogicalPropertiesTransformer, px2rem]}>
        <AntdApp>
          {/* <RouterProvider router={router} /> */}
          <App/>
        </AntdApp>
      </StyleProvider>
    </ConfigProvider>
  </Provider>
  // </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();