import React, { ReactNode, useState } from 'react';
import {
  RouterProvider
} from "react-router-dom";
import { router } from '@/router/index';

const App: React.FC = () => {
  return (
    <RouterProvider router={router} />)
};
export default App;
