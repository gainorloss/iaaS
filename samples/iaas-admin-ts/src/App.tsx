import React, { ReactNode, useState } from 'react';
import {
  createBrowserRouter,
  createRoutesFromElements,
  Route,
  RouterProvider
} from "react-router-dom";
import { RouteConfig, routes } from '@/router';
import AuthRouter from "@/router/AuthRouter";

const RouteAuthFun = (
  (routeList: RouteConfig[]) => {
    return routeList.map(
      (item: {
        path?: string;
        auth: boolean;
        element?: ReactNode;
        children?: any;
      }) => {
        return (
          <Route
            path={item.path}
            element={
              <AuthRouter auth={item.auth} key={item.path}>
                {item.element}
              </AuthRouter>
            }
            key={item.path}
          >
            {/* 递归调用，因为可能存在多级的路由 */}
            {item?.children && RouteAuthFun(item.children)}
          </Route>
        );
      }
    );
  }
);

const router = createBrowserRouter(createRoutesFromElements(RouteAuthFun(routes)));

const App: React.FC = () => {
  return (
    <RouterProvider router={router} />)
};
export default App;
