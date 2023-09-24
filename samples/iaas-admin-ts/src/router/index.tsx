import {  Route, RouteObject, createBrowserRouter, createRoutesFromElements, useNavigate} from "react-router-dom";
import Home from "../pages/Home"
import Product from "../pages/pc/spu/Product";
import Login from "../pages/Login";
import UserList from "../pages/uc/users/UserList";
import Layout from "@/pages/layout";
import NotFound from "@/pages/NotFound";
import AuthRoute from "./AuthRoute";
import { ReactNode } from "react";

export interface RouteConfig {
    path: string;
    element: React.ReactNode;
    auth: boolean;
    children?: RouteConfig[];
    redirect?: string;
}

export const routes: RouteConfig[] = [
    { path: "/login", auth: false, element: <Login /> },
    {
        path: '/',
        element: <Layout />,
        auth: true,
        children: [
            { path: '/home', element: <Home />, auth: true },
            { path: '/pc/spu', element: <Product />, auth: true },
            { path: '/uc/users', element: <UserList />, auth: true },
            { path: "*", element: <NotFound />, auth: true },
        ]
    }];

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
                            <AuthRoute auth={item.auth} key={item.path}>
                                {item.element}
                            </AuthRoute>
                        }
                        key={item.path}
                    >
                        {/* 递归调用，因为可能存在多级的路由 */}
                        {item?.children && RouteAuthFun(item.children)}
                    </Route>
                );
            }
        );
    });
export const router = createBrowserRouter(createRoutesFromElements(RouteAuthFun(routes)));