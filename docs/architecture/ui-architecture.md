# UI Architecture

## Overview

This framework uses **React micro-frontends** with **Module Federation** to enable independent development, deployment, and scaling of UI components across domains. The **Platform** provides the shell (navigation, theme, auth), while each **business domain** contributes its own UI modules.

## Core Principles

### 1. Micro-Frontend Architecture
Each domain owns its own UI:

```
Platform/src/UI/          → Shell (routing, nav, theme)
Beneficiary/src/UI/       → Beneficiary management screens
Questions/src/UI/         → Question screens
Points/src/UI/            → Points dashboard
```

**Benefits**:
- **Independent deployment**: Deploy Beneficiary UI without affecting Points UI
- **Team autonomy**: Beneficiary team owns entire stack
- **Technology flexibility**: Could use different React versions (with caution)
- **Scalability**: Scale UI components independently

### 2. Module Federation
Webpack Module Federation enables runtime composition:

```mermaid
graph TD
    Shell[Platform Shell] --> Routing[React Router]
    Shell --> Theme[MUI Theme]
    Shell --> Nav[Navigation]
    
    Routing --> BenRoute[/beneficiary/*]
    Routing --> QuesRoute[/questions/*]
    Routing --> PointsRoute[/points/*]
    
    BenRoute -.->|lazy load| BenUI[Beneficiary UI Module]
    QuesRoute -.->|lazy load| QuesUI[Questions UI Module]
    PointsRoute -.->|lazy load| PointsUI[Points UI Module]
```

**Key Feature**: UI modules loaded on-demand (lazy loading).

### 3. AcmeCorp Branding
Consistent design across all modules:

**Colors**:
- Primary Blue: `#0072CE`
- Secondary Orange: `#FF6B35`
- Success Green: `#28A745`
- Error Red: `#DC3545`

**Typography**: Roboto (Material-UI default)

---

## Platform Shell (Host Application)

### Responsibility
The Platform UI provides:
- **Routing**: Top-level routes for all domains
- **Navigation**: Header, sidebar, menu
- **Theme**: MUI theme with AcmeCorp branding
- **Authentication**: Login, logout, user context
- **SignalR**: Real-time notification hub
- **Shared Components**: Layout, error boundaries, loading states

### Project Structure

```
Platform/src/UI/
├── public/
│   └── acmecorp-logo.svg
├── src/
│   ├── components/
│   │   ├── layout/
│   │   │   ├── Header.tsx
│   │   │   ├── Sidebar.tsx
│   │   │   └── Footer.tsx
│   │   └── common/
│   │       ├── Loading.tsx
│   │       ├── ErrorBoundary.tsx
│   │       └── Notification.tsx
│   ├── pages/
│   │   ├── Dashboard.tsx
│   │   └── NotFound.tsx
│   ├── theme/
│   │   └── theme.ts
│   ├── App.tsx
│   ├── index.tsx
│   └── routes.tsx
├── package.json
└── webpack.config.js
```

### App.tsx (Root Component)

```typescript
// Platform/src/UI/src/App.tsx
import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { ThemeProvider } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { theme } from './theme/theme';
import Header from './components/layout/Header';
import Sidebar from './components/layout/Sidebar';
import Dashboard from './pages/Dashboard';
import NotFound from './pages/NotFound';

// Lazy load domain modules
const BeneficiaryModule = React.lazy(() => import('beneficiary/BeneficiaryApp'));
const QuestionsModule = React.lazy(() => import('questions/QuestionsApp'));
const PointsModule = React.lazy(() => import('points/PointsApp'));

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <BrowserRouter>
        <Header />
        <Sidebar />
        <React.Suspense fallback={<div>Loading...</div>}>
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/beneficiary/*" element={<BeneficiaryModule />} />
            <Route path="/questions/*" element={<QuestionsModule />} />
            <Route path="/points/*" element={<PointsModule />} />
            <Route path="*" element={<NotFound />} />
          </Routes>
        </React.Suspense>
      </BrowserRouter>
    </ThemeProvider>
  );
}

export default App;
```

### Theme Configuration

```typescript
// Platform/src/UI/src/theme/theme.ts
import { createTheme } from '@mui/material/styles';

export const theme = createTheme({
  palette: {
    primary: {
      main: '#0072CE',  // AcmeCorp Blue
      light: '#4A9FE8',
      dark: '#005199',
      contrastText: '#FFFFFF'
    },
    secondary: {
      main: '#FF6B35',  // AcmeCorp Orange
      light: '#FF8C5E',
      dark: '#CC5529',
      contrastText: '#FFFFFF'
    },
    success: {
      main: '#28A745'
    },
    error: {
      main: '#DC3545'
    },
    background: {
      default: '#F5F5F5',
      paper: '#FFFFFF'
    }
  },
  typography: {
    fontFamily: '"Roboto", "Helvetica", "Arial", sans-serif',
    h1: {
      fontSize: '2.5rem',
      fontWeight: 500
    },
    h2: {
      fontSize: '2rem',
      fontWeight: 500
    },
    h3: {
      fontSize: '1.75rem',
      fontWeight: 500
    }
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 8,
          textTransform: 'none'
        }
      }
    },
    MuiCard: {
      styleOverrides: {
        root: {
          borderRadius: 12,
          boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
        }
      }
    }
  }
});
```

### Header Component

```typescript
// Platform/src/UI/src/components/layout/Header.tsx
import React from 'react';
import { AppBar, Toolbar, Typography, IconButton, Box } from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import { useNavigate } from 'react-router-dom';

interface HeaderProps {
  onMenuClick?: () => void;
}

const Header: React.FC<HeaderProps> = ({ onMenuClick }) => {
  const navigate = useNavigate();
  
  return (
    <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
      <Toolbar>
        <IconButton
          color="inherit"
          edge="start"
          onClick={onMenuClick}
          sx={{ mr: 2 }}
        >
          <MenuIcon />
        </IconButton>
        
        <Box
          component="img"
          src="/acmecorp-logo.svg"
          alt="AcmeCorp"
          sx={{ height: 40, mr: 2, cursor: 'pointer' }}
          onClick={() => navigate('/')}
        />
        
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          AcmeCorp Migration Platform
        </Typography>
        
        {/* User menu, notifications, etc. */}
      </Toolbar>
    </AppBar>
  );
};

export default Header;
```

### Sidebar/Navigation

```typescript
// Platform/src/UI/src/components/layout/Sidebar.tsx
import React from 'react';
import { Drawer, List, ListItem, ListItemIcon, ListItemText, Divider } from '@mui/material';
import DashboardIcon from '@mui/icons-material/Dashboard';
import PeopleIcon from '@mui/icons-material/People';
import QuizIcon from '@mui/icons-material/Quiz';
import StarsIcon from '@mui/icons-material/Stars';
import { useNavigate } from 'react-router-dom';

const drawerWidth = 240;

const Sidebar: React.FC = () => {
  const navigate = useNavigate();
  
  const menuItems = [
    { text: 'Dashboard', icon: <DashboardIcon />, path: '/' },
    { text: 'Beneficiaries', icon: <PeopleIcon />, path: '/beneficiary' },
    { text: 'Questions', icon: <QuizIcon />, path: '/questions' },
    { text: 'Points', icon: <StarsIcon />, path: '/points' }
  ];
  
  return (
    <Drawer
      variant="permanent"
      sx={{
        width: drawerWidth,
        flexShrink: 0,
        '& .MuiDrawer-paper': {
          width: drawerWidth,
          boxSizing: 'border-box',
          top: 64  // Below header
        }
      }}
    >
      <List>
        {menuItems.map((item) => (
          <ListItem
            button
            key={item.text}
            onClick={() => navigate(item.path)}
          >
            <ListItemIcon>{item.icon}</ListItemIcon>
            <ListItemText primary={item.text} />
          </ListItem>
        ))}
      </List>
      <Divider />
    </Drawer>
  );
};

export default Sidebar;
```

---

## Domain UI Modules

### Beneficiary UI Structure

```
Beneficiary/src/UI/
├── src/
│   ├── components/
│   │   ├── BeneficiaryList.tsx
│   │   ├── BeneficiaryDetail.tsx
│   │   └── BulkImport.tsx
│   ├── pages/
│   │   ├── BeneficiaryDashboard.tsx
│   │   ├── BulkUploadPage.tsx
│   │   └── BeneficiaryDetailPage.tsx
│   ├── utils/
│   │   ├── validation.ts
│   │   └── csvParser.ts
│   ├── types/
│   │   └── beneficiary.types.ts
│   ├── BeneficiaryApp.tsx
│   └── index.tsx
├── package.json
└── webpack.config.js
```

### BeneficiaryApp.tsx (Module Root)

```typescript
// Beneficiary/src/UI/src/BeneficiaryApp.tsx
import React from 'react';
import { Routes, Route } from 'react-router-dom';
import BeneficiaryDashboard from './pages/BeneficiaryDashboard';
import BulkUploadPage from './pages/BulkUploadPage';
import BeneficiaryDetailPage from './pages/BeneficiaryDetailPage';

const BeneficiaryApp: React.FC = () => {
  return (
    <Routes>
      <Route index element={<BeneficiaryDashboard />} />
      <Route path="bulk-upload" element={<BulkUploadPage />} />
      <Route path=":id" element={<BeneficiaryDetailPage />} />
    </Routes>
  );
};

export default BeneficiaryApp;
```

### Bulk Upload Page Example

```typescript
// Beneficiary/src/UI/src/pages/BulkUploadPage.tsx
import React, { useState } from 'react';
import { Box, Card, CardContent, Typography, Button } from '@mui/material';
import { useDropzone } from 'react-dropzone';
import Papa from 'papaparse';
import { BeneficiaryDto } from '../types/beneficiary.types';
import { validateBeneficiary } from '../utils/validation';

const BulkUploadPage: React.FC = () => {
  const [file, setFile] = useState<File | null>(null);
  const [data, setData] = useState<BeneficiaryDto[]>([]);
  const [errors, setErrors] = useState<string[]>([]);
  
  const onDrop = (acceptedFiles: File[]) => {
    const uploadedFile = acceptedFiles[0];
    setFile(uploadedFile);
    
    // Parse CSV
    Papa.parse(uploadedFile, {
      header: true,
      complete: (results) => {
        const parsedData = results.data as BeneficiaryDto[];
        setData(parsedData);
        
        // Validate
        const validationErrors: string[] = [];
        parsedData.forEach((row, index) => {
          const error = validateBeneficiary(row);
          if (error) {
            validationErrors.push(`Row ${index + 1}: ${error}`);
          }
        });
        setErrors(validationErrors);
      }
    });
  };
  
  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    accept: {
      'text/csv': ['.csv'],
      'application/vnd.ms-excel': ['.xls'],
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet': ['.xlsx']
    }
  });
  
  const handleSubmit = async () => {
    // Send to API
    const response = await fetch('http://localhost:7075/api/beneficiary/bulk-upload', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });
    
    if (response.ok) {
      alert('Upload successful!');
    }
  };
  
  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom>
        Bulk Beneficiary Upload
      </Typography>
      
      <Card>
        <CardContent>
          <Box
            {...getRootProps()}
            sx={{
              border: '2px dashed #0072CE',
              borderRadius: 2,
              p: 4,
              textAlign: 'center',
              cursor: 'pointer',
              bgcolor: isDragActive ? 'rgba(0, 114, 206, 0.1)' : 'transparent'
            }}
          >
            <input {...getInputProps()} />
            {isDragActive ? (
              <Typography>Drop the file here...</Typography>
            ) : (
              <Typography>
                Drag and drop a CSV/Excel file here, or click to select
              </Typography>
            )}
          </Box>
          
          {file && (
            <Box sx={{ mt: 2 }}>
              <Typography variant="h6">File: {file.name}</Typography>
              <Typography>Records: {data.length}</Typography>
              
              {errors.length > 0 && (
                <Box sx={{ mt: 2, color: 'error.main' }}>
                  <Typography variant="h6">Validation Errors:</Typography>
                  {errors.map((error, index) => (
                    <Typography key={index}>{error}</Typography>
                  ))}
                </Box>
              )}
              
              {errors.length === 0 && (
                <Button
                  variant="contained"
                  color="primary"
                  onClick={handleSubmit}
                  sx={{ mt: 2 }}
                >
                  Submit Upload
                </Button>
              )}
            </Box>
          )}
        </CardContent>
      </Card>
    </Box>
  );
};

export default BulkUploadPage;
```

---

## Module Federation Configuration

### Platform webpack.config.js (Host)

```javascript
// Platform/src/UI/webpack.config.js
const ModuleFederationPlugin = require('webpack/lib/container/ModuleFederationPlugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const path = require('path');

module.exports = {
  entry: './src/index.tsx',
  mode: 'development',
  devServer: {
    port: 3000,
    historyApiFallback: true
  },
  output: {
    publicPath: 'http://localhost:3000/'
  },
  resolve: {
    extensions: ['.ts', '.tsx', '.js', '.jsx']
  },
  module: {
    rules: [
      {
        test: /\.(ts|tsx)$/,
        use: 'ts-loader',
        exclude: /node_modules/
      }
    ]
  },
  plugins: [
    new ModuleFederationPlugin({
      name: 'platform',
      remotes: {
        beneficiary: 'beneficiary@http://localhost:3001/remoteEntry.js',
        questions: 'questions@http://localhost:3002/remoteEntry.js',
        points: 'points@http://localhost:3003/remoteEntry.js'
      },
      shared: {
        react: { singleton: true, eager: true },
        'react-dom': { singleton: true, eager: true },
        '@mui/material': { singleton: true },
        'react-router-dom': { singleton: true }
      }
    }),
    new HtmlWebpackPlugin({
      template: './public/index.html'
    })
  ]
};
```

### Beneficiary webpack.config.js (Remote)

```javascript
// Beneficiary/src/UI/webpack.config.js
const ModuleFederationPlugin = require('webpack/lib/container/ModuleFederationPlugin');

module.exports = {
  entry: './src/index.tsx',
  mode: 'development',
  devServer: {
    port: 3001,
    historyApiFallback: true
  },
  output: {
    publicPath: 'http://localhost:3001/'
  },
  resolve: {
    extensions: ['.ts', '.tsx', '.js', '.jsx']
  },
  module: {
    rules: [
      {
        test: /\.(ts|tsx)$/,
        use: 'ts-loader',
        exclude: /node_modules/
      }
    ]
  },
  plugins: [
    new ModuleFederationPlugin({
      name: 'beneficiary',
      filename: 'remoteEntry.js',
      exposes: {
        './BeneficiaryApp': './src/BeneficiaryApp.tsx'
      },
      shared: {
        react: { singleton: true },
        'react-dom': { singleton: true },
        '@mui/material': { singleton: true },
        'react-router-dom': { singleton: true }
      }
    })
  ]
};
```

**Key Points**:
- **Host (Platform)**: Defines `remotes` (domain modules)
- **Remote (Beneficiary)**: Exposes `BeneficiaryApp` component
- **Shared dependencies**: React, MUI, React Router (singleton to avoid duplicates)

---

## Component Patterns

### Container/Presenter Pattern

**Container** (smart component):
```typescript
// BeneficiaryListContainer.tsx
import React, { useEffect, useState } from 'react';
import BeneficiaryList from './BeneficiaryList';
import { BeneficiaryDto } from '../types/beneficiary.types';

const BeneficiaryListContainer: React.FC = () => {
  const [beneficiaries, setBeneficiaries] = useState<BeneficiaryDto[]>([]);
  const [loading, setLoading] = useState(true);
  
  useEffect(() => {
    fetchBeneficiaries();
  }, []);
  
  const fetchBeneficiaries = async () => {
    const response = await fetch('http://localhost:7075/api/beneficiary');
    const data = await response.json();
    setBeneficiaries(data);
    setLoading(false);
  };
  
  return <BeneficiaryList beneficiaries={beneficiaries} loading={loading} />;
};

export default BeneficiaryListContainer;
```

**Presenter** (dumb component):
```typescript
// BeneficiaryList.tsx
import React from 'react';
import { Table, TableHead, TableRow, TableCell, TableBody, CircularProgress } from '@mui/material';
import { BeneficiaryDto } from '../types/beneficiary.types';

interface BeneficiaryListProps {
  beneficiaries: BeneficiaryDto[];
  loading: boolean;
}

const BeneficiaryList: React.FC<BeneficiaryListProps> = ({ beneficiaries, loading }) => {
  if (loading) {
    return <CircularProgress />;
  }
  
  return (
    <Table>
      <TableHead>
        <TableRow>
          <TableCell>First Name</TableCell>
          <TableCell>Last Name</TableCell>
          <TableCell>Status</TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
        {beneficiaries.map((beneficiary) => (
          <TableRow key={beneficiary.id}>
            <TableCell>{beneficiary.firstName}</TableCell>
            <TableCell>{beneficiary.lastName}</TableCell>
            <TableCell>{beneficiary.caseStatus}</TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
};

export default BeneficiaryList;
```

### Custom Hooks

```typescript
// useBeneficiaries.ts
import { useState, useEffect } from 'react';
import { BeneficiaryDto } from '../types/beneficiary.types';

export const useBeneficiaries = () => {
  const [beneficiaries, setBeneficiaries] = useState<BeneficiaryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  
  useEffect(() => {
    fetchBeneficiaries();
  }, []);
  
  const fetchBeneficiaries = async () => {
    try {
      const response = await fetch('http://localhost:7075/api/beneficiary');
      if (!response.ok) throw new Error('Failed to fetch');
      const data = await response.json();
      setBeneficiaries(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };
  
  const addBeneficiary = async (beneficiary: BeneficiaryDto) => {
    const response = await fetch('http://localhost:7075/api/beneficiary', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(beneficiary)
    });
    
    if (response.ok) {
      await fetchBeneficiaries();  // Refresh list
    }
  };
  
  return { beneficiaries, loading, error, addBeneficiary, refresh: fetchBeneficiaries };
};

// Usage
const BeneficiaryListPage: React.FC = () => {
  const { beneficiaries, loading, error, refresh } = useBeneficiaries();
  
  if (loading) return <CircularProgress />;
  if (error) return <Typography color="error">{error}</Typography>;
  
  return <BeneficiaryList beneficiaries={beneficiaries} onRefresh={refresh} />;
};
```

---

## State Management

### Local State (useState)
For component-specific state:
```typescript
const [selectedId, setSelectedId] = useState<string | null>(null);
```

### Form State (React Hook Form)
```typescript
import { useForm } from 'react-hook-form';

interface BeneficiaryFormData {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
}

const BeneficiaryForm: React.FC = () => {
  const { register, handleSubmit, formState: { errors } } = useForm<BeneficiaryFormData>();
  
  const onSubmit = async (data: BeneficiaryFormData) => {
    await fetch('http://localhost:7075/api/beneficiary', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });
  };
  
  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <TextField
        label="First Name"
        {...register('firstName', { required: 'First name is required' })}
        error={!!errors.firstName}
        helperText={errors.firstName?.message}
      />
      <TextField
        label="Last Name"
        {...register('lastName', { required: 'Last name is required' })}
        error={!!errors.lastName}
        helperText={errors.lastName?.message}
      />
      <Button type="submit">Submit</Button>
    </form>
  );
};
```

### Global State (Context API)
```typescript
// AuthContext.tsx
import React, { createContext, useState, useContext } from 'react';

interface AuthContextType {
  user: User | null;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  
  const login = async (username: string, password: string) => {
    const response = await fetch('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ username, password })
    });
    const user = await response.json();
    setUser(user);
  };
  
  const logout = () => setUser(null);
  
  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
};

// Usage
const Header: React.FC = () => {
  const { user, logout } = useAuth();
  
  return (
    <AppBar>
      <Typography>{user?.name}</Typography>
      <Button onClick={logout}>Logout</Button>
    </AppBar>
  );
};
```

---

## SignalR Integration

### SignalR Hook

```typescript
// useSignalR.ts
import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

export const useSignalR = (hubUrl: string) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [connected, setConnected] = useState(false);
  
  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect()
      .build();
    
    setConnection(newConnection);
    
    newConnection.start()
      .then(() => setConnected(true))
      .catch(err => console.error('SignalR connection error:', err));
    
    return () => {
      newConnection.stop();
    };
  }, [hubUrl]);
  
  const on = (eventName: string, callback: (...args: any[]) => void) => {
    connection?.on(eventName, callback);
  };
  
  const off = (eventName: string) => {
    connection?.off(eventName);
  };
  
  return { connection, connected, on, off };
};

// Usage
const BulkUploadProgress: React.FC<{ uploadId: string }> = ({ uploadId }) => {
  const { on, off } = useSignalR('http://localhost:7071/api');
  const [progress, setProgress] = useState(0);
  
  useEffect(() => {
    on('UploadProgress', (data) => {
      if (data.uploadId === uploadId) {
        setProgress(data.percentComplete);
      }
    });
    
    return () => off('UploadProgress');
  }, [uploadId]);
  
  return <LinearProgress variant="determinate" value={progress} />;
};
```

---

## TypeScript Types

### Shared Types

```typescript
// types/beneficiary.types.ts
export interface BeneficiaryDto {
  id?: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  nationality: string;
  documentType: string;
  documentNumber: string;
  caseStatus: CaseStatus;
  email?: string;
  phone?: string;
}

export enum CaseStatus {
  Pending = 'PENDING',
  Active = 'ACTIVE',
  Completed = 'COMPLETED',
  Suspended = 'SUSPENDED'
}

export interface ValidationError {
  field: string;
  message: string;
}
```

---

## Best Practices

### 1. Component Organization
```
Good structure:
components/
├── BeneficiaryList/
│   ├── BeneficiaryList.tsx
│   ├── BeneficiaryList.test.tsx
│   └── index.ts

Bad structure:
components/
├── BeneficiaryList.tsx
├── BeneficiaryListTest.tsx
```

### 2. Props Interface
```typescript
// Good - explicit interface
interface BeneficiaryListProps {
  beneficiaries: BeneficiaryDto[];
  loading: boolean;
  onSelect: (id: string) => void;
}

// Bad - inline types
const BeneficiaryList: React.FC<{
  beneficiaries: any[];
  loading: boolean;
}> = ({ beneficiaries, loading }) => { /* */ };
```

### 3. Error Handling
```typescript
// Good - error boundary
<ErrorBoundary fallback={<ErrorPage />}>
  <BeneficiaryModule />
</ErrorBoundary>

// Good - try/catch in async
try {
  await fetchBeneficiaries();
} catch (error) {
  setError(error.message);
}
```

### 4. Loading States
```typescript
// Good - explicit loading state
if (loading) return <CircularProgress />;
if (error) return <Typography color="error">{error}</Typography>;
return <BeneficiaryList data={data} />;

// Bad - no feedback
return <BeneficiaryList data={data} />;  // Empty while loading
```

### 5. Theme Consistency
```typescript
// Good - use theme colors
<Box sx={{ bgcolor: 'primary.main', color: 'primary.contrastText' }}>

// Bad - hardcoded colors
<Box style={{ backgroundColor: '#0072CE', color: '#FFFFFF' }}>
```

---

**Next**: See [Validation Workflow Pattern](validation-workflow-pattern.md) for markdown-driven validation rules and GitHub Actions automation.
