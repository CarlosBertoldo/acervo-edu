import React, { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import {
  Sheet,
  SheetContent,
  SheetTrigger,
} from '@/components/ui/sheet';
import {
  GraduationCap,
  LayoutDashboard,
  Kanban,
  BookOpen,
  Users,
  Settings,
  LogOut,
  Menu,
  User,
  Lock,
  FileText
} from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';
import { ROUTES } from '../constants';

const Layout = ({ children }) => {
  const location = useLocation();
  const navigate = useNavigate();
  const { user, logout } = useAuth();
  const [sidebarOpen, setSidebarOpen] = useState(false);

  const navigation = [
    {
      name: 'Dashboard',
      href: ROUTES.DASHBOARD,
      icon: LayoutDashboard,
      current: location.pathname === ROUTES.DASHBOARD
    },
    {
      name: 'Kanban',
      href: ROUTES.KANBAN,
      icon: Kanban,
      current: location.pathname === ROUTES.KANBAN
    },
    {
      name: 'Cursos',
      href: ROUTES.CURSOS,
      icon: BookOpen,
      current: location.pathname === ROUTES.CURSOS
    },
    {
      name: 'Usuários',
      href: ROUTES.USUARIOS,
      icon: Users,
      current: location.pathname === ROUTES.USUARIOS
    },
    {
      name: 'Logs',
      href: ROUTES.LOGS,
      icon: FileText,
      current: location.pathname === ROUTES.LOGS
    },
    {
      name: 'Configurações',
      href: ROUTES.CONFIGURACOES,
      icon: Settings,
      current: location.pathname === ROUTES.CONFIGURACOES
    }
  ];

  const handleLogout = async () => {
    await logout();
    navigate(ROUTES.LOGIN);
  };

  const getUserInitials = (name) => {
    if (!name) return 'U';
    return name
      .split(' ')
      .map(word => word.charAt(0))
      .join('')
      .toUpperCase()
      .slice(0, 2);
  };

  const Sidebar = ({ mobile = false }) => (
    <div className={`flex flex-col h-full ${mobile ? 'w-full' : 'w-64'}`}>
      {/* Logo */}
      <div className="flex items-center px-6 py-4 border-b border-ferreira-red-100">
        <div className="flex items-center">
          <div className="bg-ferreira-red-600 p-2 rounded-lg">
            <img 
              src="/src/assets/ferreira-costa-logo-white.png" 
              alt="Ferreira Costa" 
              className="h-6 w-auto"
            />
          </div>
          <div className="ml-3">
            <h1 className="text-lg font-semibold text-ferreira-red-700">
              Acervo Educacional
            </h1>
            <p className="text-xs text-ferreira-red-500">Ferreira Costa</p>
          </div>
        </div>
      </div>

      {/* Navigation */}
      <nav className="flex-1 px-4 py-4 space-y-1">
        {navigation.map((item) => {
          const Icon = item.icon;
          return (
            <Link
              key={item.name}
              to={item.href}
              onClick={() => mobile && setSidebarOpen(false)}
              className={`
                flex items-center px-3 py-2 text-sm font-medium rounded-md transition-colors
                ${item.current
                  ? 'bg-ferreira-green-100 text-ferreira-green-700 border-r-2 border-ferreira-green-600'
                  : 'text-gray-600 hover:bg-ferreira-red-50 hover:text-ferreira-red-700'
                }
              `}
            >
              <Icon className="mr-3 h-5 w-5" />
              {item.name}
            </Link>
          );
        })}
      </nav>

      {/* User info */}
      <div className="border-t p-4">
        <div className="flex items-center">
          <Avatar className="h-8 w-8">
            <AvatarFallback className="bg-ferreira-red-100 text-ferreira-red-700 text-sm">
              {getUserInitials(user?.nome)}
            </AvatarFallback>
          </Avatar>
          <div className="ml-3 flex-1 min-w-0">
            <p className="text-sm font-medium text-gray-900 truncate">
              {user?.nome || 'Usuário'}
            </p>
            <p className="text-xs text-gray-500 truncate">
              {user?.email || ''}
            </p>
          </div>
        </div>
      </div>
    </div>
  );

  return (
    <div className="flex h-screen bg-gray-50">
      {/* Sidebar Desktop */}
      <div className="hidden lg:flex lg:flex-shrink-0">
        <div className="flex flex-col w-64 bg-white border-r border-gray-200">
          <Sidebar />
        </div>
      </div>

      {/* Sidebar Mobile */}
      <Sheet open={sidebarOpen} onOpenChange={setSidebarOpen}>
        <SheetContent side="left" className="p-0 w-64">
          <Sidebar mobile />
        </SheetContent>
      </Sheet>

      {/* Main content */}
      <div className="flex-1 flex flex-col overflow-hidden">
        {/* Top bar */}
        <header className="bg-ferreira-red-600 border-b border-ferreira-red-700 px-4 py-3">
          <div className="flex items-center justify-between">
            <div className="flex items-center">
              {/* Mobile menu button */}
              <Sheet>
                <SheetTrigger asChild>
                  <Button
                    variant="ghost"
                    size="sm"
                    className="lg:hidden mr-2 text-white hover:bg-ferreira-red-700"
                    onClick={() => setSidebarOpen(true)}
                  >
                    <Menu className="h-5 w-5" />
                  </Button>
                </SheetTrigger>
              </Sheet>

              {/* Page title */}
              <div>
                <h2 className="text-xl font-semibold text-white">
                  {navigation.find(item => item.current)?.name || 'Dashboard'}
                </h2>
              </div>
            </div>

            {/* User menu */}
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" className="relative h-8 w-8 rounded-full hover:bg-ferreira-red-700">
                  <Avatar className="h-8 w-8">
                    <AvatarFallback className="bg-white text-ferreira-red-600">
                      {getUserInitials(user?.nome)}
                    </AvatarFallback>
                  </Avatar>
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent className="w-56" align="end" forceMount>
                <div className="flex items-center justify-start gap-2 p-2">
                  <div className="flex flex-col space-y-1 leading-none">
                    <p className="font-medium">{user?.nome}</p>
                    <p className="w-[200px] truncate text-sm text-muted-foreground">
                      {user?.email}
                    </p>
                  </div>
                </div>
                <DropdownMenuSeparator />
                <DropdownMenuItem>
                  <User className="mr-2 h-4 w-4" />
                  <span>Perfil</span>
                </DropdownMenuItem>
                <DropdownMenuItem>
                  <Lock className="mr-2 h-4 w-4" />
                  <span>Alterar Senha</span>
                </DropdownMenuItem>
                <DropdownMenuSeparator />
                <DropdownMenuItem onClick={handleLogout}>
                  <LogOut className="mr-2 h-4 w-4" />
                  <span>Sair</span>
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        </header>

        {/* Page content */}
        <main className="flex-1 overflow-y-auto p-6">
          {children}
        </main>
      </div>
    </div>
  );
};

export default Layout;

