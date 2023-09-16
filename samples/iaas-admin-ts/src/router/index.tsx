import { RouteObject, useNavigate } from "react-router-dom";
import App from "../App"
import Home from "../pages/Home"
import Product from "../pages/pc/spu/Product";
import Login from "../pages/Login";
import UserList from "../pages/uc/users/UserList";
import Layout from "@/pages/layout";
import NotFound from "@/pages/NotFound";

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