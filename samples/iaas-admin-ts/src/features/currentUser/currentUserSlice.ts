import { RootState } from "@/app/store";
import { PayloadAction, createSlice } from "@reduxjs/toolkit";

const initialState: CurrentUserState = {
    isLogin: false,
    token: {
        access_token:  localStorage.getItem("access_token") as string,
        refresh_token: localStorage.getItem("refresh_token") as string,
        expires: 3600
    },
    user: {
        id: 1,
        name: localStorage.getItem("user") as string
    }
};
export interface CurrentUserState {
    isLogin: boolean;
    token: {
        access_token: string,
        refresh_token: string,
        expires: number
    },
    user: {
        id: number,
        name: string
    }
}

export const currentUserSlice = createSlice({
    name: 'currentUser',
    initialState,
    reducers: {
        login: (state, action: PayloadAction<CurrentUserState>) => {
            return {
                ...state, ...action.payload
            }
        },
        logout: state => {
            localStorage.removeItem('access_token');
            localStorage.removeItem('refresh_token');
            return {
                ...state
            }
        }
    }
});
export const { login, logout } = currentUserSlice.actions;
export const selectCurrentUser = (state: RootState) => state.currentUser;
export default currentUserSlice.reducer;