import { RouteObject, useNavigate } from "react-router-dom";
import App from "../App"
import ErrorPage from "../ErrorPage"
import Home from "../pages/Home"
import Product from "../pages/pc/spu/Product";
import Login from "../pages/Login";
import UserList from "../pages/uc/users/UserList";

const routes:RouteObject[] = [{
    element: <App />,
    // Component: App,
    // errorElement: <ErrorPage />,
    // ErrorBoundary: ErrorPage,
    children: [
        { errorElement: <ErrorPage /> }
        , { element: <Home />, index: true }
        , { path: '/home', element: <Home /> }
        , { 
            path: '/pc', 
            children:[
                {path:'/pc/spu',index:true,element:<Product />,}
            ]
         } , { 
            path: '/uc', 
            children:[
                {path:'/uc/users',index:true,element:<UserList />,}
            ]
         }
    ]
},
{
    path:"/login",
    element: <Login />
}];
export { routes }