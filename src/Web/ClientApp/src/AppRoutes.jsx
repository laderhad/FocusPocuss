import { Counter } from "./components/Counter";
import { Weather } from "./components/Weather";
import { Tasks } from "./components/Todo";
import { Home } from "./components/Home";
import { LoginPage } from "./components/api-authorization/LoginPage";
import { RegisterPage } from "./components/api-authorization/RegisterPage";
import { ProtectedRoute } from "./components/api-authorization/ProtectedRoute";
import { TasksPage } from "./pages/TasksPage";
import { TaskDetailsPage } from "./pages/TaskDetailsPage";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/weather',
    element: <ProtectedRoute><Weather /></ProtectedRoute>
  },
  {
    path: '/todo',
    element: <ProtectedRoute><Tasks /></ProtectedRoute>
  },
  {
    path: '/tasks',
    element: <ProtectedRoute><TasksPage /></ProtectedRoute>
  },
  {
    path: '/tasks/:taskId',
    element: <ProtectedRoute><TaskDetailsPage /></ProtectedRoute>
  },
  {
    path: '/login',
    element: <LoginPage />
  },
  {
    path: '/register',
    element: <RegisterPage />
  }
];

export default AppRoutes;
